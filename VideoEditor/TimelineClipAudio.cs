namespace VideoEditor;

public class TimelineClipAudio : TimelineClip, ITimelineClip, IDisposable
{
    public TimelineClipAudio(Engine engine, Timeline timeline, StreamInfo streamInfo, TimelineClipGroup group)
        : base(engine, timeline, streamInfo, group)
    {
    }


    public double TimelineStartTime
    {
        get => Convert.ToDouble(TimelineStartFrameIndex) / Timeline.SampleRate;
        set => TimelineStartFrameIndex = Convert.ToInt64(value * Timeline.SampleRate);
    }
    public double TimelineEndTime
    {
        get => Convert.ToDouble(TimelineEndFrameIndex) / Timeline.SampleRate;
        set => TimelineEndFrameIndex = Convert.ToInt64(value * Timeline.SampleRate);
    }
    public double ClipStartTime
    {
        get => Convert.ToDouble(ClipStartFrameIndex) / StreamInfo.SampleRate!.Value;
        set => ClipStartFrameIndex = Convert.ToInt64(value * StreamInfo.SampleRate!.Value);
    }
    public double ClipEndTime
    {
        get => Convert.ToDouble(ClipEndFrameIndex) / StreamInfo.SampleRate!.Value;
        set => ClipEndFrameIndex = Convert.ToInt64(value * StreamInfo.SampleRate!.Value);
    }

    public bool IsVideoClip => false;
    public bool IsAudioClip => true;

    public void Dispose()
    {
    }
}

