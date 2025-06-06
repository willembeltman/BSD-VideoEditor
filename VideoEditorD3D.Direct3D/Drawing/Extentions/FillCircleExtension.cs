using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.SharpDXExtentions;
using VideoEditorD3D.Direct3D.Vertices;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class FillCircleExtension
{
    private const int DefaultSegments = 36; // Hoe hoger, hoe ronder

    public static void FillCircle(this GraphicsLayer graphicsLayer,
                                   int centerX, int centerY,
                                   int radius,
                                   RawColor4 color,
                                   int segments = DefaultSegments)
    {
        if (radius <= 0 || segments < 3 || color.A == 0)
            return;

        var absoluteCenterX = centerX + graphicsLayer.AbsoluteLeft;
        var absoluteCenterY = centerY + graphicsLayer.AbsoluteTop;

        var centerPoint = new RawVector2(absoluteCenterX, absoluteCenterY).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);

        float angleStep = (float)(2 * Math.PI / segments);

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep;
            float angle2 = (i + 1) * angleStep;

            var p1 = new RawVector2(
                absoluteCenterX + (float)(radius * Math.Cos(angle1)),
                absoluteCenterY + (float)(radius * Math.Sin(angle1))
            ).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);

            var p2 = new RawVector2(
                absoluteCenterX + (float)(radius * Math.Cos(angle2)),
                absoluteCenterY + (float)(radius * Math.Sin(angle2))
            ).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);

            // Voeg driehoek toe: centerPoint, p1, p2
            graphicsLayer.TriangleVertices.Add(new Vertex { Position = centerPoint, Color = color });
            graphicsLayer.TriangleVertices.Add(new Vertex { Position = p1, Color = color });
            graphicsLayer.TriangleVertices.Add(new Vertex { Position = p2, Color = color });
        }
    }
}



