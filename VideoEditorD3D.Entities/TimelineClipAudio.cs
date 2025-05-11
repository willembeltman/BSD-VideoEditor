using System.ComponentModel.DataAnnotations.Schema;

namespace VideoEditorD3D.Entities;

public class TimelineClipAudio : TimelineClip
{
    [NotMapped]
    public override bool IsVideoClip => false;
    [NotMapped]
    public override bool IsAudioClip => true;
}