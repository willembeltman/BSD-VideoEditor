using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities.Interfaces
{
    public interface ITimelineClip : IEntity
    {
        long TimelineId { get; }
        long MediaStreamId { get; }
        double StartTime { get; set; }
        double EndTime { get; }
        double ClipStartTime { get; }
        double ClipEndTime { get; }
        int Layer { get; set; }
        bool IsVideoClip { get; }
        bool IsAudioClip { get; }
        long TimelineClipGroupId { get; }
        Timeline? Timeline { get; }
        MediaStream? MediaStream { get; }
        TimelineClipGroup? TimelineClipGroup { get; }

        int OldLayer { get; set; }
        double OldTimelineStartTime { get; set; }
    }
}