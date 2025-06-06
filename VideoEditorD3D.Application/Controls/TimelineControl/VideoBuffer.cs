using VideoEditorD3D.Entities;
using VideoEditorD3D.FFMpeg.CLI;

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
        double currentTime = Timeline.CurrentTime;

        double bufferStart = currentTime - 10;
        double bufferEnd = currentTime + 10;

        if (bufferStart < VideoClip.TimelineStartTime)
            bufferStart = VideoClip.TimelineStartTime;
        if (bufferEnd > VideoClip.TimelineEndTime)
            bufferEnd = VideoClip.TimelineEndTime;

        // 1. Verwijder oude frames buiten de buffer window
        var lowest = 0d;
        lock (BufferLock)
        {
            Buffer.RemoveAll(f => f.TimelineTime < bufferStart || f.TimelineTime > bufferEnd);
            if (Buffer.Count > 0)
                lowest = Buffer.Min(a => a.TimelineTime);
        }
        var reload = lowest > currentTime;

        // 2. Reset Enumerator als teruggescrubd is
        if (Enumerator != null && reload)
        {
            Enumerator.Dispose();
            Enumerator = null;
        }

        // 3. Start reader indien nodig
        if (Enumerator == null || reload)
        {
            var startTime = (bufferStart - TimelineStartTime) * VideoClip.ClipLengthTime / VideoClip.TimelineLengthTime - VideoClip.ClipStartTime;

            var reader = new FrameReader(
                VideoClip.MediaStream.Value.MediaFile.Value.FullName,
                VideoClip.MediaStream.Value.Resolution,
                VideoClip.MediaStream.Value.Fps,
                startTime);

            Enumerator = reader.GetEnumerable().GetEnumerator();
        }

        // 4. Lees frames en vul buffer
        while (Enumerator.MoveNext())
        {
            var frame = Enumerator.Current;
            var timelineTime = frame.ClipTime * VideoClip.TimelineLengthTime / VideoClip.ClipLengthTime + VideoClip.TimelineStartTime;

            if (timelineTime > bufferEnd)
                break;

            if (bufferStart <= timelineTime && timelineTime <= bufferEnd)
            {
                lock (BufferLock)
                {
                    if (!Buffer.Any(f => f.Frame.Index == frame.Index))
                    {
                        Buffer.Add(new VideoBufferFrame(frame, timelineTime));
                        if (currentTime <= timelineTime)
                            FrameAvailable.Set();
                    }
                }
            }
            else
            {
                frame.Dispose(); // direct weggooien
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
                foreach (var item in Buffer.OrderBy(a => a.TimelineTime))
                {
                    if (item.TimelineTime > targetTime) break;
                    lastFrame = item;
                }

                if (lastFrame != null)
                    return lastFrame.Frame;
            }

            // Geen frame gevonden, wacht max 100ms op nieuwe frame
            FrameAvailable.Wait(10000);
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