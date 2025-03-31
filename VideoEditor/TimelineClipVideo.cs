using VideoEditor.FF;
using VideoEditor.Static;
using VideoEditor.Types;

namespace VideoEditor;

public class TimelineClipVideo : TimelineClip, ITimelineClip, IDisposable
{
    public TimelineClipVideo(Timeline timeline, StreamInfo streamInfo, TimelineClipGroup group) : base(timeline, streamInfo, group)
    {
    }

    public double TimelineStartTime
    {
        get => Convert.ToDouble(TimelineStartFrameIndex) * Timeline.Fps.Divider / Timeline.Fps.Base;
        set => TimelineStartFrameIndex = Convert.ToInt64(value * Timeline.Fps.Base / Timeline.Fps.Divider);
    }
    public double TimelineEndTime
    {
        get => Convert.ToDouble(TimelineEndFrameIndex) * Timeline.Fps.Divider / Timeline.Fps.Base;
        set => TimelineEndFrameIndex = Convert.ToInt64(value * Timeline.Fps.Base / Timeline.Fps.Divider);
    }
    public double TimelineLengthTime
    {
        get => Convert.ToDouble(TimelineLengthFrameIndex) * Timeline.Fps.Divider / Timeline.Fps.Base;
        set => TimelineLengthFrameIndex = Convert.ToInt64(value * Timeline.Fps.Base / Timeline.Fps.Divider);
    }

    public double ClipStartTime
    {
        get => Convert.ToDouble(ClipStartFrameIndex) * StreamInfo.Fps!.Value.Divider / StreamInfo.Fps!.Value.Base;
        set => ClipStartFrameIndex = Convert.ToInt64(value * StreamInfo.Fps!.Value.Base / StreamInfo.Fps!.Value.Divider);
    }
    public double ClipEndTime
    {
        get => Convert.ToDouble(ClipEndFrameIndex) * StreamInfo.Fps!.Value.Divider / StreamInfo.Fps!.Value.Base;
        set => ClipEndFrameIndex = Convert.ToInt64(value * StreamInfo.Fps!.Value.Base / StreamInfo.Fps!.Value.Divider);
    }
    public double ClipLengthTime
    {
        get => Convert.ToDouble(ClipLengthFrameIndex) * StreamInfo.Fps!.Value.Divider / StreamInfo.Fps!.Value.Base;
        set => ClipLengthFrameIndex = Convert.ToInt64(value * StreamInfo.Fps!.Value.Base / StreamInfo.Fps!.Value.Divider);
    }

    public bool IsVideoClip => true;
    public bool IsAudioClip => false;

    Resolution CurrentResolution { get; set; }
    FFMpeg_FrameReader? Source { get; set; }


    double CurrentTime
    {
        get => Convert.ToDouble(CurrentIndex) * StreamInfo.Fps!.Value.Divider / StreamInfo.Fps!.Value.Base;
        set => CurrentIndex = Convert.ToInt64(value * StreamInfo.Fps!.Value.Base / StreamInfo.Fps!.Value.Divider);
    }

    long RelativeCurrentTimelineFrameIndex => Timeline.CurrentFrameIndex - TimelineStartFrameIndex;
    long RelativeNextTimelineFrameIndex => Timeline.NextFrameIndex - TimelineStartFrameIndex;
    long RequestedCurrentFrameIndex => RelativeCurrentTimelineFrameIndex * ClipLengthFrameIndex / TimelineLengthFrameIndex;
    long RequestedNextFrameIndex => RelativeNextTimelineFrameIndex * ClipLengthFrameIndex / TimelineLengthFrameIndex;
    double RequestedCurrentTime => Convert.ToDouble(RequestedCurrentFrameIndex) * StreamInfo.Fps!.Value.Divider / StreamInfo.Fps!.Value.Base;
    double RequestedNextTime => Convert.ToDouble(RequestedNextFrameIndex) * StreamInfo.Fps!.Value.Divider / StreamInfo.Fps!.Value.Base;

    public Frame? GetCurrentFrame()
    {
        if (Engine.DisplayControl == null) return null;
        if (StreamInfo.Fps == null) return null;

        var reload = false;
        if (Source == null) reload = true;
        else if (Engine.DisplayControl.Resolution != CurrentResolution) reload = true;
        else if (RequestedCurrentTime < CurrentTime) reload = true;
        else if (RequestedCurrentTime > CurrentTime + 5) reload = true;
        else if (Source.OutputFrameIndex > RequestedCurrentFrameIndex) reload = true;

        if (reload)
        {
            if (Source != null)
            {
                Source.Dispose();
            }

            CurrentResolution = Engine.DisplayControl.Resolution;
            CurrentTime = RequestedCurrentTime;
            if (CurrentTime < 0) return null;
            if (CurrentTime > TimelineEndTime - TimelineStartTime) return null;
            Source = new FFMpeg_FrameReader(StreamInfo.File.FullName, CurrentResolution, StreamInfo.Fps.Value, CurrentTime);
        }

        if (Source == null) return null; // kan niet

        if (Source.OutputFrameIndex < RequestedCurrentFrameIndex)
            Source.MoveNext(RequestedCurrentFrameIndex, RequestedNextFrameIndex);

        //while (!(CurrentTime <= Timeline.CurrentTime && Timeline.CurrentTime < RequestedNextTime))
        //{
        //    Source.MoveNext(CurrentClipIndex, NextTimelineTime);
        //    CurrentIndex++;
        //}
        if (Source.OutputFrame == null) return null;
        return Source.OutputFrame;
    }


    public void Dispose()
    {
        Source?.Dispose();
    }

    //bool IsRunning { get; set; }
    //Thread? Thread { get; set; }
    //public void Start()
    //{
    //    IsRunning = true;
    //    Thread = new Thread(new ThreadStart(TheThread));
    //    Thread.Start();
    //}
    //public void Dispose()
    //{
    //    IsRunning = false;
    //    if (Thread != null && Thread.CurrentThread != Thread) Thread.Join();
    //    Thread = null;
    //}
    //public void TheThread()
    //{
    //    while (Engine.IsRunning && IsRunning)
    //    {
    //        var currentTime = Timeline.CurrentTime;

    //    }
    //}

}

