using System.ComponentModel.DataAnnotations.Schema;
using VideoEditorD3D.Entities.Interfaces;
using VideoEditorD3D.FFMpeg;

namespace VideoEditorD3D.Entities;

public class TimelineClipVideo : TimelineClip
{
    [NotMapped]
    public override bool IsVideoClip => true;
    [NotMapped]
    public override bool IsAudioClip => false;
}