using VideoEditorD3D.Database.Interfaces;

namespace VideoEditorD3D.Entities;

public class TimelineVideo : IEntity
{
    public long Id { get; set; }
    public long TimelineId { get; set; }
    public long MediaStreamId { get; set; }
    public double StartTime { get; set; }
    public double EndTime { get; set; }
    public double ClipStartTime { get; set; }
    public double ClipEndTime { get; set; }
}