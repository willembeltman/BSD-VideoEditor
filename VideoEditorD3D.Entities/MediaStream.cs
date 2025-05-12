using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class MediaStream : IEntity
{
    public long Id { get; set; }
    public long MediaFileId { get; set; }
    public int Index { get; set; }
    public string Type { get; set; }

    [ForeignKey("MediaFileId")]
    public virtual Lazy<MediaFile> MediaFile { get; set; }

    [ForeignKey("MediaStreamId")]
    public virtual ICollection<TimelineClipVideo> TimelineVideos { get; set; }
}