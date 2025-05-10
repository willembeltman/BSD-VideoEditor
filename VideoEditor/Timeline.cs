using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditor;

public class Timeline : IDisposable
{
    public Timeline(Project project)
    {
        Project = project;
        Resolution = new Resolution(1920, 1080);
        Fps = new Fps(30, 1);
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

    public double VisibleWidth { get; set; } = 100;
    public double VisibleStart { get; set; } = 0;
    public int FirstVisibleVideoLayer { get; set; } = 0;
    public int VisibleVideoLayers { get; set; } = 3;
    public int FirstVisibleAudioLayer { get; set; } = 0;
    public int VisibleAudioLayers { get; set; } = 3;

    public long CurrentFrameIndex { get; set; } = 0;
    public double CurrentTime
    {
        get => Convert.ToDouble(CurrentFrameIndex) * Fps.Divider / Fps.Base;
        set => CurrentFrameIndex = Convert.ToInt64(value * Fps.Base / Fps.Divider);
    }
    public TimeStamp CurrentTimeStamp
    {
        get => new TimeStamp(CurrentTime);
        set => CurrentTime = value.TotalSeconds;
    }

    public long NextFrameIndex => CurrentFrameIndex + 1;
    public double NextTime
    {
        get => Convert.ToDouble(NextFrameIndex) * Fps.Divider / Fps.Base;
        set => CurrentFrameIndex = Convert.ToInt64(value * Fps.Base / Fps.Divider) - 1;
    }
    public TimeStamp NextTimeStamp
    {
        get => new TimeStamp(NextTime);
        set => NextTime = value.TotalSeconds;
    }

    public TimelineClipVideo[] CurrentVideoClips => VideoClips
        .Where(a => a.TimelineStartTime <= CurrentTime && CurrentTime < a.TimelineEndTime)
        .ToArray();

    public void Dispose()
    {
        foreach (var videoclip in VideoClips)
            videoclip.Dispose();
        foreach (var audioclip in AudioClips)
            audioclip.Dispose();
    }
}
