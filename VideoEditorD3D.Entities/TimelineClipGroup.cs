using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities;

public class TimelineClipGroup : IEntity
{
    public long Id { get; set; }
    public long TimelineId { get; set; }

    [ForeignKey("TimelineId")]
    public Lazy<Timeline?> Timeline { get; set; } = new Lazy<Timeline?>(() => null, true);

    [ForeignKey("TimelineClipGroupId")]
    public ICollection<TimelineClipVideo?> TimelineVideos { get; set; } = [];
    [ForeignKey("TimelineClipGroupId")]
    public ICollection<TimelineClipAudio?> TimelineAudios { get; set; } = [];

    public bool IsEqualTo(TimelineClipGroup group)
    {
        return this == group;
    }
}
