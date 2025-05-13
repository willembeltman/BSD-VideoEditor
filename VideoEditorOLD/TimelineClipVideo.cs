using VideoEditorD3D.FFMpeg;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditor;

public class TimelineClipVideo : TimelineClip, ITimelineClip, IDisposable
{
    public TimelineClipVideo(Timeline timeline, StreamInfo streamInfo, TimelineClipGroup group) 
        : base(timeline, streamInfo, group)
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
    FrameReader? Source { get; set; }

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

    public Frame? GetCurrentFrame(Resolution resolution)
    {
        if (StreamInfo.Fps == null) return null;

        var reload = false;
        if (Source == null) reload = true;
        else if (CurrentResolution != resolution) reload = true;
        else if (RequestedCurrentTime < CurrentTime) reload = true;
        else if (RequestedCurrentTime > CurrentTime + 5) reload = true;
        else if (Source.CurrentFrameIndex > RequestedCurrentFrameIndex) reload = true;

        if (reload)
        {
            if (Source != null)
            {
                Source.Dispose();
            }

            CurrentResolution = resolution;
            CurrentTime = RequestedCurrentTime;
            if (CurrentTime < 0) return null;
            if (CurrentTime > TimelineEndTime - TimelineStartTime) return null;
            Source = new FrameReader(StreamInfo.File.FullName, CurrentResolution, StreamInfo.Fps.Value, CurrentTime);
        }

        if (Source == null) return null; // kan niet

        if (Source.CurrentFrameIndex < RequestedCurrentFrameIndex)
            Source.MoveNext(RequestedCurrentFrameIndex, RequestedNextFrameIndex);

        if (Source.CurrentFrame == null) return null;
        return Source.CurrentFrame;
    }

    public void Dispose()
    {
        Source?.Dispose();
    }
}

