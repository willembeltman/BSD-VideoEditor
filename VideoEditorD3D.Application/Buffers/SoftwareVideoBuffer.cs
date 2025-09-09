using System.Collections.Concurrent;
using VideoEditorD3D.Entities;
using VideoEditorD3D.FFMpeg.CLI;
using VideoEditorD3D.FFMpeg.Interfaces;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.Application.Buffers;

public class SoftwareVideoBuffer : IVideoBuffer
{
    private readonly ManualResetEventSlim FrameAvailable;
    private readonly object BufferLock;
    private readonly Timeline Timeline;
    private readonly TimelineClipVideo VideoClip;
    private readonly AutoResetEvent CurrentTimeUpdated;
    private ConcurrentDictionary<long, SoftwareVideoBufferFrame> Buffer;
    private IEnumerator<IVideoFrame>? Enumerator;
    private readonly Thread Thread;
    private bool KillSwitch;

    public double TimelineStartTime => VideoClip.TimelineStartTime;
    public double TimelineEndTime => VideoClip.TimelineEndTime;
    public int TimelineLayer => VideoClip.TimelineLayer;

    public SoftwareVideoBuffer(Timeline timeline, TimelineClipVideo videoClip)
    {
        Timeline = timeline;
        VideoClip = videoClip;
        Buffer = new ConcurrentDictionary<long, SoftwareVideoBufferFrame>();
        Thread = new Thread(new ThreadStart(Kernel));
        BufferLock = new object();
        CurrentTimeUpdated = new AutoResetEvent(false);
        FrameAvailable = new ManualResetEventSlim(false);

        Timeline.CurrentTimeUpdated += Timeline_CurrentTimeUpdated;
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
        var lowest = double.MaxValue;

        var removeBuffers = new List<KeyValuePair<long, SoftwareVideoBufferFrame>>();
        foreach (var buffer in Buffer)
        {
            var timelineTime = buffer.Value.TimelineTime;
            if (timelineTime < bufferStart || timelineTime > bufferEnd)
            {
                removeBuffers.Add(buffer);
            }
            else if (timelineTime < lowest)
            {
                lowest = timelineTime;
            }
        }
        foreach (var buffer in removeBuffers)
        {
            Buffer.TryRemove(buffer);
            buffer.Value.Dispose();
        }
        var reload = lowest > currentTime;

        // 2. Reset Enumerator als teruggescrubd is
        if (Enumerator != null && reload)
        {
            Enumerator.Dispose();
            Enumerator = null;
        }

        // 3. Start reader indien nodig
        if (Enumerator == null)
        {
            var startTime = (bufferStart - TimelineStartTime) * VideoClip.ClipLengthTime / VideoClip.TimelineLengthTime - VideoClip.ClipStartTime;

            var reader = new VideoFrameReader(
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

            var frameClipTime = VideoClip.MediaStream.Value.Fps.ConvertIndexToTime(frame.Index);
            var timelineTime = frameClipTime * VideoClip.TimelineLengthTime / VideoClip.ClipLengthTime + VideoClip.TimelineStartTime;

            if (timelineTime > bufferEnd)
                break;

            if (bufferStart <= timelineTime && timelineTime <= bufferEnd)
            {
                lock (BufferLock)
                {
                    if (!Buffer.Any(f => f.Value.Frame.Index == frame.Index))
                    {
                        Buffer[frame.Index] = new SoftwareVideoBufferFrame(frame, timelineTime);
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

    public IVideoFrame GetCurrentFrame(double targetTime)
    {
        //for (var i = 0; i < 3; i++)
        while (true) 
        {
            SoftwareVideoBufferFrame? lastFrame = null;
            foreach (var item in Buffer.Values.OrderBy(a => a.TimelineTime))
            {
                if (item.TimelineTime > targetTime) break;
                lastFrame = item;
            }

            if (lastFrame != null)
                return lastFrame.Frame;

            // Geen frame gevonden, wacht max 100ms op nieuwe frame
            FrameAvailable.Wait(10000);
            FrameAvailable.Reset(); // Daarna resetten
        }
        throw new NotImplementedException();
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
            item.Value.Dispose();
        }
        Buffer.Clear();
        FrameAvailable.Dispose();


        Timeline.CurrentTimeUpdated -= Timeline_CurrentTimeUpdated;
        GC.SuppressFinalize(this);
    }

    public class SoftwareVideoBufferFrame : IDisposable
    {
        public IVideoFrame Frame { get; }
        public double TimelineTime { get; }
        public SoftwareVideoBufferFrame(IVideoFrame frame, double timelineTime)
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