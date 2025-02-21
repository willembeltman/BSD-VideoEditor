namespace VideoEditor;

public class Project
{
    public Project()
    {
        CurrentTimeline = new Timeline(this);
        Timelines.Add(CurrentTimeline);
    }

    public ConcurrentArray<File> Files { get; } = [];
    public ConcurrentArray<Timeline> Timelines { get; } = [];

    public string? Path { get; set; }
    public Timeline CurrentTimeline { get; set; }
}