using EntityFrameworkZip.Interfaces;

namespace VideoEditorD3D.Entities;

public class TimelineClipGroup : IEntity
{
    public long Id { get; set; }
    public long TimelineId { get; set; }

    public virtual Lazy<Timeline> Timeline { get; set; }

    public virtual ICollection<TimelineClipVideo> TimelineVideos { get; set; }
    public virtual ICollection<TimelineClipAudio> TimelineAudios { get; set; }

    public bool IsEqualTo(TimelineClipGroup group)
    {
        return this == group;
    }
}
