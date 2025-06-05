using EntityFrameworkZip;

namespace VideoEditorD3D.Entities;

public class TimelineClipGroup : IEntity
{
    public long Id { get; set; }
    public long? TimelineId { get; set; }

    public long? MediaFileId { get; set; }

    public virtual ILazy<Timeline> Timeline { get; set; }

    public virtual ICollection<TimelineClipVideo> TimelineClipVideos { get; set; }
    public virtual ICollection<TimelineClipAudio> TimelineClipAudios { get; set; }

    public bool IsEqualTo(TimelineClipGroup group)
    {
        return this == group;
    }
}
