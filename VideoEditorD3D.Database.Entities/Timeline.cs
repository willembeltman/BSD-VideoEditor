using VideoEditorD3D.Database.Interfaces;
using VideoEditorD3D.Types;

namespace VideoEditorD3D.Database.Entities;

public class Timeline : IEntity
{
    public long Id { get; set; }
    public Fps Fps { get; set; }
    public Resolution Resolution { get; set; }
}
