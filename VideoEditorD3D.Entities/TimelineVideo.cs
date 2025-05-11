using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class TimelineVideo : IEntity, ITimelineClip
{
    public long Id { get; set; }
    public long TimelineId { get; set; }
    public long MediaStreamId { get; set; }
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public double ClipStartTime { get; set; }
    public double ClipEndTime { get; set; }

    [ForeignKey("TimelineId")]
    public virtual Timeline? TimelineItem { get; set; }
    [ForeignKey("MediaStreamId")]
    public virtual MediaStream? MediaStream { get; set; }
}