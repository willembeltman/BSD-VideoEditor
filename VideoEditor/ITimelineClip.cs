namespace VideoEditor;

public interface ITimelineClip
{
    Timeline Timeline { get; }
    StreamInfo StreamInfo { get; }
    TimelineClipGroup Group { get; set; }
    int Layer { get; set; }
    long TimelineStartIndex { get; set; }
    long TimelineEndIndex { get; set; }
    long ClipStartIndex { get; set; }
    long ClipEndIndex { get; set; }
    double TimelineStartTime { get; set; }
    double TimelineEndTime { get; set; }
    double ClipStartTime { get; set; }
    double ClipEndTime { get; set; }
    bool IsVideoClip { get; }
    bool IsAudioClip { get; }
    double OldTimelineStartInSeconds { get; set; }
    double OldTimelineEndInSeconds { get; set; }
    int OldLayer { get; set; }
}