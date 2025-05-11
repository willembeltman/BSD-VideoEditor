using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.Entities;

public class Timeline : IEntity
{
    public long Id { get; set; }
    public long ProjectId { get; set; }

    public bool IsCurrent { get; set; }
    public Fps Fps { get; set; } = new Fps();
    public Resolution Resolution { get; set; } = new Resolution();

    [ForeignKey("ProjectId")]
    public Lazy<Project?> Project { get; set; } = new Lazy<Project?>(() => null, true);

    [ForeignKey("TimelineId")]
    public virtual ICollection<TimelineVideo> TimelineVideos { get; set; } = [];
    [ForeignKey("TimelineId")]
    public virtual ICollection<TimelineAudio> TimelineAudios { get; set; } = [];
}