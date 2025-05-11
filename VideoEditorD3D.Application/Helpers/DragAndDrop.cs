using VideoEditorD3D.Entities;

namespace VideoEditorD3D.Application.Helpers;

public class DragAndDrop
{
    public List<MediaFile> MediaFiles { get; } = [];
    public List<TimelineVideo> VideoClips { get; } = [];
    public List<TimelineAudio> AudioClips { get; } = [];
    public IEnumerable<ITimelineClip> AllClips =>
        VideoClips
            .Select(a => a as ITimelineClip)
            .Concat(AudioClips);

    public void Clear()
    {
        MediaFiles.Clear();
        AudioClips.Clear();
        VideoClips.Clear();
    }
}
