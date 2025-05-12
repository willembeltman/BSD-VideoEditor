using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class MediaFile : IEntity
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string FullName { get; set; }
    
    public virtual Lazy<Project> Project { get; set; } 
    public virtual ICollection<MediaStream> MediaStreams { get; set; }
}
