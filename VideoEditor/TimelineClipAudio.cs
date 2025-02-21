namespace VideoEditor;

public class TimelineClipAudio : TimelineClip, ITimelineClip
{
    public TimelineClipAudio(Timeline timeline, StreamInfo streamInfo, TimelineClipGroup group) : base(timeline, streamInfo, group)
    {
    }


    public double TimelineStartInSeconds
    {
        get => Convert.ToDouble(TimelineStartIndex) / Timeline.SampleRate;
        set => TimelineStartIndex = Convert.ToInt64(value * Timeline.SampleRate);
    }
    public double TimelineEndInSeconds
    {
        get => Convert.ToDouble(TimelineEndIndex) / Timeline.SampleRate;
        set => TimelineEndIndex = Convert.ToInt64(value * Timeline.SampleRate);
    }
    public double ClipStartInSeconds
    {
        get => Convert.ToDouble(ClipStartIndex) / StreamInfo.SampleRate!.Value;
        set => ClipStartIndex = Convert.ToInt64(value * StreamInfo.SampleRate!.Value);
    }
    public double ClipEndInSeconds
    {
        get => Convert.ToDouble(ClipEndIndex) / StreamInfo.SampleRate!.Value;
        set => ClipEndIndex = Convert.ToInt64(value * StreamInfo.SampleRate!.Value);
    }

    public bool IsVideoClip => false;
    public bool IsAudioClip => true;

}

