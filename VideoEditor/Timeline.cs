using VideoEditor.Types;
namespace VideoEditor;

public class Timeline
{
    public Timeline(Project project)
    {
        Project = project;
        Resolution = new Resolution(1920, 1080);
        Fps = new Fps(25, 1);
        SampleRate = 48000;
        AudioChannels = 2;
    }

    public Project Project { get; }

    public ConcurrentArray<TimelineClipVideo> VideoClips { get; } = [];
    public ConcurrentArray<TimelineClipAudio> AudioClips { get; } = [];
    public IEnumerable<ITimelineClip> AllClips => VideoClips.Select(a => a as ITimelineClip).Concat(AudioClips);
    public ConcurrentArray<ITimelineClip> SelectedClips { get; } = [];
    public ConcurrentArray<TimelineClipGroup> ClipGroups { get; } = [];

    public Resolution Resolution { get; set; }
    public Fps Fps { get; set; }
    public int SampleRate { get; set; }
    public int AudioChannels { get; set; }

    public double PlayerPosition { get; set; } = 0;
    public double VisibleWidth { get; set; } = 100;
    public double VisibleStart { get; set; } = 0;
    public int FirstVisibleVideoLayer { get; set; } = 0;
    public int VisibleVideoLayers { get; set; } = 3;
    public int FirstVisibleAudioLayer { get; set; } = 0;
    public int VisibleAudioLayers { get; set; } = 3;
}
