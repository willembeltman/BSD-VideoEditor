using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class Project : IEntity
{
    public long Id { get; set; }
    public long CurrentTimelineId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual ICollection<MediaFile> Files { get; set; }

    [ForeignKey("ProjectId")]
    public virtual ICollection<Timeline> Timelines { get; set; }

    [ForeignKey("CurrentTimelineId")]
    public virtual Lazy<Timeline> CurrentTimeline { get; set; }

}