using SharpDX.Direct3D11;

namespace VideoEditorD3D.Direct3D.Interfaces;

public interface IApplicationD3D
{
    Device Device { get; }
    Characters Characters { get; }
    int Width { get; }
    int Height { get; }
}