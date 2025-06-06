using VideoEditorD3D.Entities;

namespace VideoEditorD3D.Application.Controls.TimelineControl;

public class DragAndDrop
{
    public List<MediaFile> MediaFiles { get; } = [];
    public List<MediaStream> MediaStreams { get; } = [];
    public List<TimelineClipVideo> VideoClips { get; } = [];
    public List<TimelineClipAudio> AudioClips { get; } = [];
    public IEnumerable<TimelineClip> AllClips =>
        VideoClips
            .Select(a => a as TimelineClip)
            .Concat(AudioClips);

    public void Clear()
    {
        MediaFiles.Clear();
        MediaStreams.Clear();
        AudioClips.Clear();
        VideoClips.Clear();
    }
}
