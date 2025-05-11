using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class Project : IEntity
{
    public long Id { get; set; }

    [ForeignKey("ProjectId")]
    public ICollection<MediaFile> Files { get; set; } = [];
    [ForeignKey("ProjectId")]
    public ICollection<Timeline> Timelines { get; set; } = [];

    public long CurrentTimelineId { get; set; }

    [ForeignKey("CurrentTimelineId")]
    public Lazy<Timeline?> CurrentTimeline { get; set; } = new Lazy<Timeline?>(() => null, true);

}