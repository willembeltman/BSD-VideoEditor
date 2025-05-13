using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Vertices;

namespace VideoEditorD3D.Direct3D.Drawing;

internal static class CreateTextureVerticesForRectangleExtention
{
    internal static TextureVertex[] CreateTextureVerticesForRectangle(this GraphicsLayer graphicsLayer, int left, int top, int width, int height)
    {
        // Afronden op hele pixels
        var roundLeft = Convert.ToSingle(left);
        var roundTop = Convert.ToSingle(top);
        var roundWidth = Convert.ToSingle(width);
        var roundHeight = Convert.ToSingle(height);
        var roundRight = left + roundWidth;
        var roundBottom = top + roundHeight;

        // Maak vertices
        float x0 = roundLeft / graphicsLayer.Width * 2f - 1f;
        float y0 = 1f - roundTop / graphicsLayer.Height * 2f;
        float x1 = roundRight / graphicsLayer.Width * 2f - 1f;
        float y1 = 1f - roundBottom / graphicsLayer.Height * 2f;

        var FillVertices = new[]
        {
            new TextureVertex { Position = new RawVector2(x0, y0), UV = new RawVector2(0, 0) },
            new TextureVertex { Position = new RawVector2(x1, y0), UV = new RawVector2(1, 0) },
            new TextureVertex { Position = new RawVector2(x0, y1), UV = new RawVector2(0, 1) },
            new TextureVertex { Position = new RawVector2(x1, y0), UV = new RawVector2(1, 0) },
            new TextureVertex { Position = new RawVector2(x1, y1), UV = new RawVector2(1, 1) },
            new TextureVertex { Position = new RawVector2(x0, y1), UV = new RawVector2(0, 1) },
        };
        return FillVertices;
    }
}