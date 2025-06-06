using EntityFrameworkZip;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.Entities;

public class MediaStream : IEntity
{
    public long Id { get; set; }
    public long? MediaFileId { get; set; }

    public int Index { get; set; }
    public bool IsVideo { get; set; }
    public bool IsAudio { get; set; }

    public Resolution Resolution { get; set; }
    public Fps Fps { get; set; }

    public int? SampleRate { get; set; }
    public int? Channels { get; set; }

    public virtual ILazy<MediaFile> MediaFile { get; set; } = new LazyStatic<MediaFile>();
    public virtual ICollection<TimelineClipVideo> TimelineClipVideos { get; set; } = new List<TimelineClipVideo>(); 
    public virtual ICollection<TimelineClipAudio> TimelineClipAudios { get; set; } = new List<TimelineClipAudio>();
}