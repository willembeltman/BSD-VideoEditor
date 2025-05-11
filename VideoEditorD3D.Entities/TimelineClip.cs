using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.Interfaces;
using VideoEditorD3D.FFMpeg;

namespace VideoEditorD3D.Entities;

public abstract class TimelineClip : ITimelineClip
{
    public long Id { get; set; }
    public long TimelineId { get; set; }
    public long MediaStreamId { get; set; }
    public long TimelineClipGroupId { get; set; }
    public double StartTime { get; set; }
    public double LengthTime { get; set; }
    public double EndTime
    {
        get => StartTime + LengthTime; 
        set
        {
            LengthTime = value - StartTime;
            if (LengthTime < 0)
            {
                LengthTime = 0;
                StartTime = value;
            }
        }
    }
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
    public int Layer { get; set; }

    [ForeignKey("TimelineId")]
    public Lazy<Timeline?> Timeline { get; set; } = new Lazy<Timeline?>(() => null, true);
    [ForeignKey("MediaStreamId")]
    public Lazy<MediaStream?> MediaStream { get; set; } = new Lazy<MediaStream?>(() => null, true);
    [ForeignKey("TimelineClipGroupId")]
    public Lazy<TimelineClipGroup?> TimelineClipGroup { get; set; } = new Lazy<TimelineClipGroup?>(() => null, true);

    [NotMapped]
    public StreamInfo StreamInfo { get; set; }
    [NotMapped]
    public abstract bool IsVideoClip { get; }
    [NotMapped]
    public abstract bool IsAudioClip { get; }
    [NotMapped]
    public int OldLayer { get; set; }
    [NotMapped]
    public double OldTimelineStartTime { get; set; }

    Timeline? ITimelineClip.Timeline => Timeline.Value;
    MediaStream? ITimelineClip.MediaStream => MediaStream.Value;
    TimelineClipGroup? ITimelineClip.TimelineClipGroup => TimelineClipGroup.Value;


}