using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class MediaFile : IEntity
{
    public long Id { get; set; }
    public long ProjectId { get; set; }
    public string? FullName { get; set; }
    
    [ForeignKey("ProjectId")]
    public Lazy<Project?> Project { get; set; } = new Lazy<Project?>(() => null, true);

    [ForeignKey("MediaFileId")]
    public ICollection<MediaStream> MediaStreams { get; set; } = [];
}
