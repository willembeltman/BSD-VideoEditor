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


    /// <summary>
    /// Reads frames from a video file and writes them into a double-buffered system for frame processing.
    /// The reader continuously pulls frames from the video file, storing them in one of the two available buffers. 
    /// It waits for a signal to start reading and notifies when the next frame is ready. 
    /// The process automatically terminates when the end of the video is reached or if an error occurs.
    /// </summary>
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

            while (!KillSwitch)
            {
                ReadNextFrameEvent.WaitOne();

                var read = 0;
                while (!KillSwitch && read < NextFrame.Buffer.Length)
                {
                    var taken = stream.Read(NextFrame.Buffer, read, NextFrame.Buffer.Length - read);
                    if (taken <= 0)
                    {
                        NextEndOfVideo = true;
                        break;
                    }
                    read += taken;
                }
                if (KillSwitch || NextEndOfVideo) break;

                NextFrame.Index = NextFrameIndex;
                NextFrameIndex++;
                
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

    /// <summary>
    /// Advances the reader to the next frame using double buffering.
    /// If the next frame has already been read, it is returned immediately.
    /// Otherwise, the function will block until the background reader has finished loading it.
    /// Once retrieved, the buffers are swapped, and the reader is signaled to start loading 
    /// the following frame in the background.
    /// </summary>
    /// <returns>The next available frame.</returns>
    public Frame? GetFrame(long frameIndex, long nextFrameIndex)
    {
        // Check end of video of exception
        if (EndOfVideo || KillSwitch) return null;

        // Check if requested frame index is smaller or equal to next requested frame index
        if (frameIndex > nextFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"{nameof(frameIndex)} cannot be larger then {nameof(nextFrameIndex)}");

        // Check if requested frame index has already been read
        if (frameIndex < CurrentFrameIndex)
            throw new ArgumentOutOfRangeException(
                $"{nameof(frameIndex)} cannot be smaller then {nameof(CurrentFrameIndex)}");

        // Set the requested frameIndex
        RequestedFrameIndex = frameIndex;

        // Start if needed
        if (!IsStarted)
        {
            IsStarted = true;
            ReaderWorker.Start();
        }

        // Seek till the frame index has reached the requested frame index
        while (CurrentFrameIndex < RequestedFrameIndex)
        {
            // Wait for the reader to finish
            NextFrameReadyEvent.WaitOne();

            // Switch buffers
            FrameSwitch = !FrameSwitch;

            // Signal reader to read next frame
            ReadNextFrameEvent.Set();

            // Kill if end of video (the switch of the buffer could have caused this)
            if (EndOfVideo || KillSwitch) return null;
        }

        // Set requested frame index to the next frame index
        RequestedFrameIndex = nextFrameIndex;

        // Signal reader to read next frame
        ReadNextFrameEvent.Set();

        // Return the current frame
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
