using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class MediaStream : IEntity
{
    public long Id { get; set; }
    public long MediaFileId { get; set; }
    public int Index { get; set; }
    public string? Type { get; set; }

    [ForeignKey("MediaFileId")]
    public Lazy<MediaFile?> MediaFile { get; set; } = new Lazy<MediaFile?>(() => null, true);

    [ForeignKey("MediaStreamId")]
    public ICollection<TimelineClipVideo> TimelineVideos { get; set; } = [];
}