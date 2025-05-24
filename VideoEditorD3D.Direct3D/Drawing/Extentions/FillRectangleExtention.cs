using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.SharpDXExtentions;
using VideoEditorD3D.Direct3D.Vertices;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class FillRectangleExtention
{
    public static void FillRectangle(this GraphicsLayer graphicsLayer, int left, int top, int width, int height, RawColor4 color)
    {
        var absoluteLeft = left + graphicsLayer.AbsoluteLeft;
        var absoluteTop = top + graphicsLayer.AbsoluteTop;
        var right = absoluteLeft + width;
        var bottom = absoluteTop + height;

        // Clip space coordinaten
        var p1 = new RawVector2(absoluteLeft, absoluteTop).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);
        var p2 = new RawVector2(right, absoluteTop).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);
        var p3 = new RawVector2(right, bottom).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);
        var p4 = new RawVector2(absoluteLeft, bottom).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);

        // Vulling met 2 driehoeken
        if (color.A > 0)
        {
            graphicsLayer.TriangleVertices.Add(new Vertex { Position = p1, Color = color });
            graphicsLayer.TriangleVertices.Add(new Vertex { Position = p2, Color = color });
            graphicsLayer.TriangleVertices.Add(new Vertex { Position = p3, Color = color });

            graphicsLayer.TriangleVertices.Add(new Vertex { Position = p3, Color = color });
            graphicsLayer.TriangleVertices.Add(new Vertex { Position = p4, Color = color });
            graphicsLayer.TriangleVertices.Add(new Vertex { Position = p1, Color = color });
        }
    }
}
