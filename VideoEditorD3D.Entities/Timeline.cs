using VideoEditorD3D.Entities.ZipDatabase.Interfaces;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.Entities;

public class Timeline : IEntity
{
    public long Id { get; set; }
    public Fps Fps { get; set; }
    public Resolution Resolution { get; set; }
}
