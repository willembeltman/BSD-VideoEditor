using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.SharpDXExtentions;
using VideoEditorD3D.Direct3D.Vertices;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class DrawLineOnePixelWideExtention
{
    public static void DrawLineOnePixelWide(this GraphicsLayer graphicsLayer, int startX, int startY, int endX, int endY, RawColor4 color)
    {
        RawVector2 start = new RawVector2(startX + graphicsLayer.AbsoluteLeft, startY + graphicsLayer.AbsoluteTop);
        RawVector2 end = new RawVector2(endX + graphicsLayer.AbsoluteLeft, endY + graphicsLayer.AbsoluteTop);
        graphicsLayer.LineVertices.Add(new Vertex { Position = start.ToClipSpace(graphicsLayer.Width, graphicsLayer.Height), Color = color });
        graphicsLayer.LineVertices.Add(new Vertex { Position = end.ToClipSpace(graphicsLayer.Width, graphicsLayer.Height), Color = color });
    }
}
