using VideoEditorD3D.FFMpeg;

namespace VideoEditor;

public class TimelineClip
{
    public TimelineClip(Timeline timeline, StreamInfo streamInfo, TimelineClipGroup group)
    {
        Timeline = timeline;
        StreamInfo = streamInfo;
        Group = group;
    }

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