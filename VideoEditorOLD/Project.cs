using MediaContainer = VideoEditorD3D.FFMpeg.MediaContainer;

namespace VideoEditor;

public class Project : IDisposable
{
    public Project()
    {
        CurrentTimeline = new Timeline(this);
        Timelines.Add(CurrentTimeline);
    }

    public ConcurrentArray<MediaContainer> Files { get; } = [];
    public ConcurrentArray<Timeline> Timelines { get; } = [];

    public string? Path { get; set; }
    public Timeline CurrentTimeline { get; set; }

    public void Dispose()
    {
        foreach (var timeline in Timelines)
            timeline.Dispose();
    }
}