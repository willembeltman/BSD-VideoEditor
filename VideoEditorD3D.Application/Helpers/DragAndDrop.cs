using VideoEditorD3D.Entities;
using VideoEditorD3D.Entities.Interfaces;
using VideoEditorD3D.FFMpeg;

namespace VideoEditorD3D.Application.Helpers;

public class DragAndDrop
{
    public List<MediaContainer> MediaContainers { get; } = [];
    public List<TimelineClipVideo> VideoClips { get; } = [];
    public List<TimelineClipAudio> AudioClips { get; } = [];
    public IEnumerable<ITimelineClip> AllClips =>
        VideoClips
            .Select(a => a as ITimelineClip)
            .Concat(AudioClips);

    public void Clear()
    {
        MediaContainers.Clear();
        AudioClips.Clear();
        VideoClips.Clear();
    }
}
