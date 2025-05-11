using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.Interfaces;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.Entities;

public class Timeline : IEntity
{
    public long Id { get; set; }
    public long ProjectId { get; set; }

    public Fps Fps { get; set; } = new Fps(1, 60);
    public Resolution Resolution { get; set; } = new Resolution(1920, 1080);
    public int SampleRate { get; set; } = 48000;
    public int AudioChannels { get; set; } = 2;

    public double VisibleWidth { get; set; } = 100;
    public double VisibleStart { get; set; } = 0;
    public int FirstVisibleVideoLayer { get; set; } = 0;
    public int VisibleVideoLayers { get; set; } = 3;
    public int FirstVisibleAudioLayer { get; set; } = 0;
    public int VisibleAudioLayers { get; set; } = 3;
    public double CurrentTime { get; set; }

    [NotMapped]
    public IEnumerable<ITimelineClip> AllClips => VideoClips.Select(a => a as ITimelineClip).Concat(AudioClips);
    [NotMapped]
    public List<ITimelineClip> SelectedClips { get; } = new List<ITimelineClip>();

    [ForeignKey("ProjectId")]
    public Lazy<Project?> Project { get; set; } = new Lazy<Project?>(() => null, true);

    [ForeignKey("TimelineId")]
    public virtual ICollection<TimelineClipVideo> VideoClips { get; set; } = [];
    [ForeignKey("TimelineId")]
    public virtual ICollection<TimelineClipAudio> AudioClips { get; set; } = [];
}