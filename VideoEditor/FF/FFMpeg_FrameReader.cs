using System.Diagnostics;
using VideoEditor.Types;
namespace VideoEditor.FF;

public class FFMpeg_FrameReader : IDisposable
{
    public FFMpeg_FrameReader(string fullName, Resolution resolution, Fps fps, double startTime)
    {
        FullName = fullName;
        Resolution = resolution;
        Fps = fps;
        StartTime = startTime;
        StartTimeStamp = new TimeStamp(startTime);
        ReaderWorker = new Thread(new ThreadStart(FrameReader));

        Frame1 = new Frame(Resolution, CurrentFrameIndex);
        Frame2 = new Frame(Resolution, NextFrameIndex);
        NextFrameReadyEvent = new AutoResetEvent(false);
        ReadNextFrameEvent = new AutoResetEvent(true);
    }

    public string FullName { get; }
    public Resolution Resolution { get; }
    public Fps Fps { get; }
    public double StartTime { get; }
    public TimeStamp StartTimeStamp { get; }
    public Exception? ReaderException { get; private set; }

    private readonly Thread ReaderWorker;
    private readonly Frame Frame1;
    private readonly Frame Frame2;
    private readonly AutoResetEvent NextFrameReadyEvent;
    private readonly AutoResetEvent ReadNextFrameEvent;

    private volatile bool IsStarted;
    private volatile bool FrameSwitch;
    private volatile bool KillSwitch;
    private volatile bool EndOfVideo1;
    private volatile bool EndOfVideo2;
    private volatile bool NextSet = true;

    public long CurrentFrameIndex { get; set; } = -1;
    public Frame CurrentFrame => FrameSwitch ? Frame2 : Frame1;
    public bool EndOfVideo => FrameSwitch ? EndOfVideo2 : EndOfVideo1;

    private long NextRequestedFrameIndex { get; set; } = 0;
    private long NextFrameIndex { get; set; }
    private Frame NextFrame => FrameSwitch ? Frame1 : Frame2;
    private bool NextEndOfVideo
    {
        get => FrameSwitch ? EndOfVideo1 : EndOfVideo2;
        set
        {
            if (FrameSwitch)
                EndOfVideo1 = value;
            else
                EndOfVideo2 = value;
        }
    }

    private void FrameReader()
    {
        try
        {
            var arguments = $"-i \"{FullName}\" " +
                            $"-ss {StartTimeStamp} " +
                            $"-s {Resolution.Width}x{Resolution.Height} " +
                            $"-pix_fmt rgba -f rawvideo -";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = FFExecutebles.FFMpeg.FullName,
                WorkingDirectory = FFExecutebles.FFMpeg.Directory!.FullName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo) ?? throw new Exception("Cannot create process");
            using var stream = process.StandardOutput.BaseStream;

            while (!KillSwitch && !NextEndOfVideo)
            {
                ReadNextFrameEvent.WaitOne();

                while (NextRequestedFrameIndex > NextFrameIndex)
                {
                    if (!Read(stream)) break;
                }

                NextFrameReadyEvent.Set();
            }
        }
        catch (Exception ex)
        {
            ReaderException = ex;
            KillSwitch = true;
            NextEndOfVideo = true;
        }
        finally
        {
            NextFrameReadyEvent.Set();
        }
    }

    private bool Read(Stream stream)
    {
        if (KillSwitch || NextEndOfVideo) return false;

        var read = 0;
        while (!KillSwitch && !NextEndOfVideo && read < NextFrame.Buffer.Length)
        {
            var partialread = stream.Read(NextFrame.Buffer, read, NextFrame.Buffer.Length - read);
            read += partialread;
            NextEndOfVideo = partialread <= 0;
        }

        if (KillSwitch || NextEndOfVideo) return false;

        NextFrame.Index = NextFrameIndex;
        NextFrameIndex++;

        return true;
    }

    public Frame? MoveNext(long frameIndex, long nextFrameIndex)
    {
        Debug.WriteLine($"{frameIndex} => {nextFrameIndex}");

        if (EndOfVideo || KillSwitch) return null;

        if (frameIndex > nextFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"{nameof(frameIndex)} cannot be larger then {nameof(nextFrameIndex)}");

        if (frameIndex < CurrentFrameIndex)
        {
            throw new ArgumentOutOfRangeException(
                $"Requesting past frame");
        }

        if (frameIndex > CurrentFrameIndex && frameIndex < NextFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"Requesting future frame while already read too far");

        if (nextFrameIndex < NextFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"Already read too far");

        if (!IsStarted)
        {
            IsStarted = true;
            ReaderWorker.Start();
        }

        if (frameIndex == CurrentFrameIndex) return CurrentFrame;
        
        if (frameIndex == NextRequestedFrameIndex)
        {
            if (NextSet)
            {
                NextFrameReadyEvent.WaitOne();
                NextSet = false;
            }
            SwitchBuffer();
        }
        else if (frameIndex > NextRequestedFrameIndex)
        {
            if (NextSet)
            {
                NextFrameReadyEvent.WaitOne();
                NextSet = false;
            }
            NextRequestedFrameIndex = frameIndex;
            ReadNextFrameEvent.Set();
            // Helaas moeten we nu wachten
            NextFrameReadyEvent.WaitOne();
            SwitchBuffer();
        }

        if (NextRequestedFrameIndex < nextFrameIndex)
        {
            NextRequestedFrameIndex = nextFrameIndex;
            ReadNextFrameEvent.Set();
            NextSet = true;
        }

        if (EndOfVideo || KillSwitch) return null;
        return CurrentFrame;
    }

    private void SwitchBuffer()
    {
        FrameSwitch = !FrameSwitch;
        CurrentFrameIndex = NextFrameIndex;
    }

    public void Dispose()
    {
        KillSwitch = true;
        ReadNextFrameEvent.Set();
        NextFrameReadyEvent.Set();
        if (ReaderWorker.IsAlive && ReaderWorker != Thread.CurrentThread)
            ReaderWorker.Join();
    }
}
