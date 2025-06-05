using VideoEditorD3D.Entities;
using VideoEditorD3D.FFMpeg;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.Application.Controls.TimelineControl;

public class VideoBuffer : IDisposable
{
    private readonly ManualResetEventSlim FrameAvailable;
    private readonly object BufferLock;
    private readonly Timeline Timeline;
    private readonly TimelineClipVideo VideoClip;
    private readonly AutoResetEvent CurrentTimeUpdated;
    private List<VideoBufferFrame> Buffer;
    private IEnumerator<Frame>? Enumerator;
    private readonly Thread Thread;

    public bool KillSwitch { get; private set; }
    public double LastTime { get; private set; }

    public double TimelineStartTime => VideoClip.TimelineStartTime;
    public double TimelineEndTime => VideoClip.TimelineEndTime;
    public double ClipStartTime => VideoClip.ClipStartTime;
    public double ClipEndTime => VideoClip.ClipEndTime;
    public int Layer => VideoClip.Layer;    

    public VideoBuffer(Timeline timeline, TimelineClipVideo videoClip)
    {
        Timeline = timeline;
        Timeline.CurrentTimeUpdated += Timeline_CurrentTimeUpdated;
        VideoClip = videoClip;
        LastTime = videoClip.ClipStartTime;
        CurrentTimeUpdated = new AutoResetEvent(false); 
        Buffer = new List<VideoBufferFrame>();
        Thread = new Thread(new ThreadStart(Kernel));
        BufferLock = new object();
        FrameAvailable = new ManualResetEventSlim(false);
    }

    private void Timeline_CurrentTimeUpdated(object? sender, double e)
    {
        CurrentTimeUpdated.Set();
    }

    public void StartThread()
    {
        Thread.Start();
    }

    private void Kernel()
    {
        UpdateBuffer();
        while (!KillSwitch)
        {
            if (CurrentTimeUpdated.WaitOne(100)) // Wait for the current time to be updated
            {
                UpdateBuffer();
            }
        }
    }

    private void UpdateBuffer()
    {
        double bufferStart = Timeline.CurrentTime - 10;
        double bufferEnd = Timeline.CurrentTime + 10;
        if (bufferStart < 0)
            bufferStart = 0;
        if (bufferEnd > VideoClip.MediaStream.Value.Length) 
            bufferEnd = VideoClip.MediaStream.Value.Length;

        // 1. Verwijder oude frames buiten de buffer window
        Buffer.RemoveAll(f => f.TimelineTime < bufferStart || f.TimelineTime > bufferEnd);

        // 2. Start reader indien nodig
        if (Enumerator == null)
        {
            var reader = new SimpleFrameReader(
                VideoClip.MediaStream.Value.MediaFile.Value.FullName,
                VideoClip.TempStreamInfo.Resolution!.Value,
                VideoClip.TempStreamInfo.Fps!.Value,
                LastTime);

            Enumerator = reader.GetEnumerable().GetEnumerator();
        }

        // 3. Lees frames en vul buffer
        while (Enumerator.MoveNext())
        {
            var frame = Enumerator.Current;
            var timelineTime = frame.ClipTime * VideoClip.TimelineLengthTime / VideoClip.ClipLengthTime;

            LastTime = timelineTime;

            if (timelineTime > bufferEnd)
                break; // Stop, de rest is te ver in de toekomst

            if (timelineTime >= bufferStart && timelineTime <= bufferEnd)
            {
                // Vermijd duplicaten
                lock (BufferLock)
                {
                    if (!Buffer.Any(f => f.Frame.Index == frame.Index))
                    {
                        Buffer.Add(new VideoBufferFrame(frame, timelineTime));
                        FrameAvailable.Set(); // Signaleer dat een frame beschikbaar is
                    }
                }
            }
            else
            {
                // Niet in buffer window, dispose meteen
                frame.Dispose();
            }
        }
    }
    public Frame GetCurrentFrame()
    {
        double targetTime = Timeline.CurrentTime;

        for (int i = 0; i < 10; i++)
        {
            lock (BufferLock)
            {
                VideoBufferFrame? lastFrame = null;
                foreach (var item in Buffer)
                {
                    if (item.TimelineTime > targetTime) break;
                    lastFrame = item;
                }

                if (lastFrame != null)
                    return lastFrame.Frame;
            }

            // Geen frame gevonden, wacht max 100ms op nieuwe frame
            FrameAvailable.Wait(100);
            FrameAvailable.Reset(); // Daarna resetten
        }

        throw new Exception("No frame available for current time");
    }



    public void Dispose()
    {
        KillSwitch = true;
        if (Thread != null && Thread != Thread.CurrentThread && Thread.ThreadState == ThreadState.Running)
        {
            Thread.Join();
        }
        foreach (var item in Buffer)
        {
            item.Dispose();
        }
        Buffer.Clear(); 
        FrameAvailable.Dispose();


        Timeline.CurrentTimeUpdated -= Timeline_CurrentTimeUpdated;
        GC.SuppressFinalize(this);
    }

    public class VideoBufferFrame : IDisposable
    {
        public Frame Frame { get; }
        public double TimelineTime { get; }
        public VideoBufferFrame(Frame frame, double timelineTime)
        {
            Frame = frame;
            TimelineTime = timelineTime;
        }
        public void Dispose()
        {
            Frame.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}