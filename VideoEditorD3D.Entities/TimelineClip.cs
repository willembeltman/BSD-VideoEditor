using EntityFrameworkZip;
using VideoEditorD3D.FFMpeg;

namespace VideoEditorD3D.Entities;

public abstract class TimelineClip : IEntity
{
    public long Id { get; set; }
    public long? TimelineId { get; set; }
    public long? MediaStreamId { get; set; }
    public long? TimelineClipGroupId { get; set; }

    public double TimelineStartTime { get; set; }
    public double TimelineLengthTime { get; set; }
    public double TimelineEndTime
    {
        get => TimelineStartTime + TimelineLengthTime;
        set
        {
            TimelineLengthTime = value - TimelineStartTime;
            if (TimelineLengthTime < 0)
            {
                TimelineLengthTime = 0;
                TimelineStartTime = value;
            }
        }
    }
    public int TimelineLayer { get; set; }

    public double ClipStartTime { get; set; }
    public double ClipLengthTime { get; set; }
    public double ClipEndTime
    {
        get => ClipStartTime + ClipLengthTime;
        set
        {
            ClipLengthTime = value - ClipStartTime;
            if (ClipLengthTime < 0)
            {
                ClipLengthTime = 0;
                ClipStartTime = value;
            }
        }
    }

    public virtual ILazy<Timeline> Timeline { get; set; } = new LazyStatic<Timeline>();
    public virtual ILazy<MediaStream> MediaStream { get; set; } = new LazyStatic<MediaStream>();
    public virtual ILazy<TimelineClipGroup> TimelineClipGroup { get; set; } = new LazyStatic<TimelineClipGroup>();

    [NotMapped]
    public abstract bool IsVideoClip { get; }
    [NotMapped]
    public abstract bool IsAudioClip { get; }

    //[NotMapped]
    //public StreamInfo TempStreamInfo { get; set; }
    //[NotMapped]
    //public MediaContainer TempMediaContainer { get; set; }
    [NotMapped]
    public int OldLayer { get; set; }
    [NotMapped]
    public double OldTimelineStartTime { get; set; }
}