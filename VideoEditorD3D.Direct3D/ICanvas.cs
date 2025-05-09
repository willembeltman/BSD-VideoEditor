using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D;

public interface ICanvas
{
    IEnumerable<CanvasLayer> GetCanvasLayers();
    RawColor4 BackgroundColor { get; }
}