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
        Frame1 = new Frame(Resolution, OutputFrameIndex);
        Frame2 = new Frame(Resolution, ReaderFrameIndex);
        NextFrameReadyEvent = new AutoResetEvent(false);
        ReadNextFrameEvent = new AutoResetEvent(true);

        ReaderTime = startTime;
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

    public long RequestedFrameIndex { get; private set; }

    public long OutputFrameIndex => ReaderFrameIndex - 1;
    public double OutputTime => Convert.ToDouble(OutputFrameIndex) * Fps.Divider / Fps.Base;
    public Frame OutputFrame => FrameSwitch ? Frame2 : Frame1;
    public bool OutputEndOfVideo => FrameSwitch ? EndOfVideo2 : EndOfVideo1;

    private long ReaderFrameIndex { get; set; }
    private double ReaderTime
    {
        get => Convert.ToDouble(ReaderFrameIndex) * Fps.Divider / Fps.Base;
        set => ReaderFrameIndex = Convert.ToInt64(value * Fps.Base / Fps.Divider);
    }
    private Frame ReaderFrame => FrameSwitch ? Frame1 : Frame2;
    private bool ReaderEndOfVideo
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

            while (!KillSwitch && !ReaderEndOfVideo)
            {
                ReadNextFrameEvent.WaitOne();

                while (RequestedFrameIndex >= ReaderFrameIndex)
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
            ReaderEndOfVideo = true;
        }
        finally
        {
            NextFrameReadyEvent.Set();
        }
    }

    private bool Read(Stream stream)
    {
        if (KillSwitch || ReaderEndOfVideo) return false;

        var read = 0;
        while (!KillSwitch && !ReaderEndOfVideo && read < ReaderFrame.Buffer.Length)
        {
            var partialread = stream.Read(ReaderFrame.Buffer, read, ReaderFrame.Buffer.Length - read);
            read += partialread;
            ReaderEndOfVideo = partialread <= 0;
        }

        if (KillSwitch || ReaderEndOfVideo) return false;

        ReaderFrame.Index = ReaderFrameIndex;
        ReaderFrameIndex++;

        return true;
    }

    public Frame? MoveNext(long frameIndex, long nextFrameIndex)
    {
        if (OutputEndOfVideo || KillSwitch) return null;

        if (frameIndex > nextFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"{nameof(frameIndex)} cannot be larger then {nameof(nextFrameIndex)}");

        if (frameIndex < OutputFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"{nameof(frameIndex)} cannot be smaller then {nameof(OutputFrameIndex)}");

        if (nextFrameIndex < ReaderFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"{nameof(nextFrameIndex)} cannot be smaller then {nameof(ReaderFrameIndex)}");

        if (!IsStarted)
        {
            IsStarted = true;
            ReaderWorker.Start();
        }

        if (frameIndex != OutputFrameIndex)
        {
            NextFrameReadyEvent.WaitOne();

            FrameSwitch = !FrameSwitch;
            if (OutputEndOfVideo || KillSwitch) return null;

            if (OutputFrameIndex < frameIndex)
            {
                RequestedFrameIndex = frameIndex;
                ReadNextFrameEvent.Set();
                NextFrameReadyEvent.WaitOne();

                FrameSwitch = !FrameSwitch;
                if (OutputEndOfVideo || KillSwitch) return null;
            }
        }

        if (RequestedFrameIndex < nextFrameIndex)
        {
            RequestedFrameIndex = nextFrameIndex;
            ReadNextFrameEvent.Set();
        }

        return OutputFrame;
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
