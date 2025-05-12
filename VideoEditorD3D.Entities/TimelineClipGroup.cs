using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class TimelineClipGroup : IEntity
{
    public long Id { get; set; }
    public long TimelineId { get; set; }

    [ForeignKey("TimelineId")]
    public virtual Lazy<Timeline> Timeline { get; set; }

    [ForeignKey("TimelineClipGroupId")]
    public virtual ICollection<TimelineClipVideo> TimelineVideos { get; set; }
    [ForeignKey("TimelineClipGroupId")]
    public virtual ICollection<TimelineClipAudio> TimelineAudios { get; set; }

    public bool IsEqualTo(TimelineClipGroup group)
    {
        return this == group;
    }
}
