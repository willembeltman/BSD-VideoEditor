using EntityFrameworkZip;

namespace VideoEditorD3D.Entities;

public class MediaStream : IEntity
{
    public long Id { get; set; }
    public long? MediaFileId { get; set; }

    public int Index { get; set; }
    public double Length { get; set; }

    public virtual ILazy<MediaFile> MediaFile { get; set; }
    public virtual ICollection<TimelineClipVideo> TimelineClipVideos { get; set; }
    public virtual ICollection<TimelineClipAudio> TimelineClipAudios { get; set; }
}