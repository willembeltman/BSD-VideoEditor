using System.Diagnostics;
using VideoEditor.Types;
namespace VideoEditor.FF;

public class FFMpeg_FrameReader : IDisposable
{
    public FFMpeg_FrameReader(string fullName, Resolution resolution, double startTime = 0, long startIndex = 0)
    {
        FullName = fullName;
        Resolution = resolution;
        StartTime = startTime;
        StartTimeStamp = new TimeStamp(startTime);
        ReaderWorker = new Thread(new ThreadStart(FrameReader));
        Frame1 = new Frame(Resolution, NextFrameIndex);
        Frame2 = new Frame(Resolution, NextFrameIndex);
        NextFrameReadyEvent = new AutoResetEvent(false);
        ReadNextFrameEvent = new AutoResetEvent(true);
        NextFrameIndex = startIndex;
    }

    public string FullName { get; }
    public Resolution Resolution { get; }
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

    private long NextFrameIndex;
    private long RequestedFrameIndex;

    public Frame CurrentFrame => FrameSwitch ? Frame1 : Frame2;
    public long CurrentFrameIndex => NextFrameIndex - 1;
    public bool EndOfVideo => FrameSwitch ? EndOfVideo1 : EndOfVideo2;
    private Frame NextFrame => FrameSwitch ? Frame2 : Frame1;
    private bool NextEndOfVideo
    {
        get => FrameSwitch ? EndOfVideo2 : EndOfVideo1;
        set
        {
            if (FrameSwitch)
                EndOfVideo2 = value;
            else
                EndOfVideo1 = value;
        }
    }
    
    private void FrameReader()
    {
        try
        {
            var arguments = $"-i \"{FullName}\" " +
                            $"-ss {StartTimeStamp} " +
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

                while (RequestedFrameIndex >= NextFrameIndex)
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

    public Frame? GetFrame(long frameIndex, long nextFrameIndex)
    {
        if (EndOfVideo || KillSwitch) return null;

        if (frameIndex > nextFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"{nameof(frameIndex)} cannot be larger then {nameof(nextFrameIndex)}");

        if (frameIndex < CurrentFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"{nameof(frameIndex)} cannot be smaller then {nameof(CurrentFrameIndex)}");

        if (nextFrameIndex < NextFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"{nameof(nextFrameIndex)} cannot be smaller then {nameof(NextFrameIndex)}");

        if (!IsStarted)
        {
            IsStarted = true;
            ReaderWorker.Start();
        }

        if (frameIndex != CurrentFrameIndex)
        {
            NextFrameReadyEvent.WaitOne();

            FrameSwitch = !FrameSwitch;
            if (EndOfVideo || KillSwitch) return null;

            if (CurrentFrameIndex < frameIndex)
            {
                RequestedFrameIndex = frameIndex;
                ReadNextFrameEvent.Set();
                NextFrameReadyEvent.WaitOne();

                FrameSwitch = !FrameSwitch;
                if (EndOfVideo || KillSwitch) return null;
            }
        }

        if (RequestedFrameIndex < nextFrameIndex)
        {
            RequestedFrameIndex = nextFrameIndex;
            ReadNextFrameEvent.Set();
        }

        return CurrentFrame;
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
