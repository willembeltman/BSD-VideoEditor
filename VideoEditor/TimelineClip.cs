namespace VideoEditor;

public class TimelineClip
{
    public TimelineClip(Engine engine, Timeline timeline, StreamInfo streamInfo, TimelineClipGroup group)
    {
        Engine = engine;
        Timeline = timeline;
        StreamInfo = streamInfo;
        Group = group;
    }

    public Engine Engine { get; }
    public Timeline Timeline { get; }
    public StreamInfo StreamInfo { get; }
    public int Layer { get; set; }
    public long CurrentIndex { get; set; }
    public long TimelineStartFrameIndex { get; set; }
    public long TimelineLengthFrameIndex { get; set; }
    public long ClipStartFrameIndex { get; set; }
    public long ClipLengthFrameIndex { get; set; }
    public double OldTimelineStartTime { get; set; }
    //public double OldTimelineLengthInSeconds { get; set; }
    public int OldLayer { get; set; }
    public TimelineClipGroup Group { get; set; }

    public long TimelineEndFrameIndex
    {
        get => TimelineStartFrameIndex + TimelineLengthFrameIndex;
        set => TimelineLengthFrameIndex = value - TimelineStartFrameIndex;
    }
    public long ClipEndFrameIndex
    {
        get => ClipStartFrameIndex + ClipLengthFrameIndex;
        set => TimelineLengthFrameIndex = value - TimelineStartFrameIndex;
    }
}