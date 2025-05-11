using MediaContainer = VideoEditorD3D.FFMpeg.MediaContainer;

namespace VideoEditor.Types;

public class DragAndDrop
{
    public List<MediaContainer> Files { get; } = [];
    public List<TimelineClipVideo> VideoClips { get; } = [];
    public List<TimelineClipAudio> AudioClips { get; } = [];
    public IEnumerable<ITimelineClip> AllClips =>
        VideoClips
            .Select(a => a as ITimelineClip)
            .Concat(AudioClips);

    public void Clear()
    {
        Files.Clear();
        AudioClips.Clear();
        VideoClips.Clear();
    }
}
