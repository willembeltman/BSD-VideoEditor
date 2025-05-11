using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.Entities;

public class Timeline : IEntity
{
    public long Id { get; set; }
    public bool IsCurrent { get; set; }
    public Fps Fps { get; set; }
    public Resolution Resolution { get; set; }

    [ForeignKey("TimelineId")]
    public virtual ICollection<TimelineVideo> TimelineVideos { get; set; } = [];
    [ForeignKey("TimelineId")]
    public virtual ICollection<TimelineAudio> TimelineAudios { get; set; } = [];
}