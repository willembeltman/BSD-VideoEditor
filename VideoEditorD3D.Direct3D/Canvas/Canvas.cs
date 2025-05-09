using Color = SharpDX.Color;

namespace VideoEditorD3D.Direct3D.Canvas
{
    public class Canvas : IDisposable
    {
        public Canvas(CanvasLayer[] layers, Color? backgroundColor = null)
        {
            Layers = layers;
            BackgroundColor = backgroundColor ?? new Color(0, 0, 0, 1);
        }

        public CanvasLayer[] Layers { get; }
        public Color BackgroundColor { get; }

        public void Dispose()
        {
            foreach (var layer in Layers)
                layer.Dispose();
        }
    }
}