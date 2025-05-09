using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D;

public class Canvas
{
    public Canvas(IEnumerable<CanvasLayer> layers, RawColor4? backgroundColor = null)
    {
        Layers = layers;
        BackgroundColor = backgroundColor ?? new RawColor4(0, 0, 0, 1);
    }

    public IEnumerable<CanvasLayer> Layers { get; }
    public RawColor4 BackgroundColor { get; }
}