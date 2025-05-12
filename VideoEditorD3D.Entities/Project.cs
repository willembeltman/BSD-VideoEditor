using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class Project : IEntity
{
    public MyClass myClass { get; set; }

    public long Id { get; set; }
    public long CurrentTimelineId { get; set; }

    public virtual ICollection<MediaFile> Files { get; set; }
    public virtual ICollection<Timeline> Timelines { get; set; }
    public virtual Lazy<Timeline> CurrentTimeline { get; set; }
}

public struct MyClass
{
}