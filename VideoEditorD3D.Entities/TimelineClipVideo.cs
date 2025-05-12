using System.ComponentModel.DataAnnotations.Schema;

namespace VideoEditorD3D.Entities;

public class TimelineClipVideo : TimelineClip
{
    [NotMapped]
    public override bool IsVideoClip => true;
    [NotMapped]
    public override bool IsAudioClip => false;
}