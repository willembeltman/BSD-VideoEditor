using VideoEditorD3D.Entities;
using VideoEditorD3D.FFMpeg;

namespace VideoEditorD3D.Application.Controls.TimelineControl;

public class DragAndDrop
{
    public List<MediaContainer> MediaContainers { get; } = [];
    public List<TimelineClipVideo> VideoClips { get; } = [];
    public List<TimelineClipAudio> AudioClips { get; } = [];
    public IEnumerable<TimelineClip> AllClips =>
        VideoClips
            .Select(a => a as TimelineClip)
            .Concat(AudioClips);

    public void Clear()
    {
        MediaContainers.Clear();
        AudioClips.Clear();
        VideoClips.Clear();
    }
}
