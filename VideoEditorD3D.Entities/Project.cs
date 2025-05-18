using EntityFrameworkZip;

namespace VideoEditorD3D.Entities;

public class Project : IEntity
{
    public long Id { get; set; }
    public long CurrentTimelineId { get; set; }

    public virtual ICollection<MediaFile> Files { get; set; }
    public virtual ICollection<Timeline> Timelines { get; set; }
    public virtual ILazy<Timeline> CurrentTimeline { get; set; }

    //public Lazy<Timeline> CurrentTimeline2 { get; set; }
}
