using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.Interfaces;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class TimelineAudio : IEntity, ITimelineClip
{
    public long Id { get; set; }
    public long TimelineId { get; set; }
    public long MediaStreamId { get; set; }
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public double ClipStartTime { get; set; }
    public double ClipEndTime { get; set; }

    [ForeignKey("TimelineId")]
    public Lazy<Timeline?> TimelineItem { get; set; } = new Lazy<Timeline?>(() => null, true);
    [ForeignKey("MediaStreamId")]
    public Lazy<MediaStream?> MediaStream { get; set; } = new Lazy<MediaStream?>(() => null, true);
}