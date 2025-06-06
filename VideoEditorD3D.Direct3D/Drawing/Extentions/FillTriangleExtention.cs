using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.SharpDXExtentions;
using VideoEditorD3D.Direct3D.Vertices;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class FillTriangleExtension
{
    public static void FillTriangle(this GraphicsLayer graphicsLayer,
                                    int x1, int y1,
                                    int x2, int y2,
                                    int x3, int y3,
                                    RawColor4 color)
    {
        if (color.A == 0)
            return;

        var absoluteX1 = x1 + graphicsLayer.AbsoluteLeft;
        var absoluteY1 = y1 + graphicsLayer.AbsoluteTop;
        var absoluteX2 = x2 + graphicsLayer.AbsoluteLeft;
        var absoluteY2 = y2 + graphicsLayer.AbsoluteTop;
        var absoluteX3 = x3 + graphicsLayer.AbsoluteLeft;
        var absoluteY3 = y3 + graphicsLayer.AbsoluteTop;

        var p1 = new RawVector2(absoluteX1, absoluteY1).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);
        var p2 = new RawVector2(absoluteX2, absoluteY2).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);
        var p3 = new RawVector2(absoluteX3, absoluteY3).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);

        graphicsLayer.TriangleVertices.Add(new Vertex { Position = p1, Color = color });
        graphicsLayer.TriangleVertices.Add(new Vertex { Position = p2, Color = color });
        graphicsLayer.TriangleVertices.Add(new Vertex { Position = p3, Color = color });
    }
}



