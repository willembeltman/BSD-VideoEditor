using EntityFrameworkZip;

namespace VideoEditorD3D.Entities;

public class MediaFile : IEntity
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string FullName { get; set; }

    public virtual ILazy<Project> Project { get; set; }
    public virtual ICollection<MediaStream> MediaStreams { get; set; }
}
