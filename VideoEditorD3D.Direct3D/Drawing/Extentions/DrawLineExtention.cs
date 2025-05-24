using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.SharpDXExtentions;
using VideoEditorD3D.Direct3D.Vertices;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class DrawLineExtention
{
    public static void DrawLine(this GraphicsLayer graphicsLayer, int startX, int startY, int endX, int endY, RawColor4 color, int strokeWidth = 1)
    {
        if (strokeWidth < 1)
            return;

        if (strokeWidth == 1)
        {
            graphicsLayer.DrawLineOnePixelWide(startX, startY, endX, endY, color);
            return;
        }

        RawVector2 start = new RawVector2(startX + graphicsLayer.AbsoluteLeft, startY + graphicsLayer.AbsoluteTop);
        RawVector2 end = new RawVector2(endX + graphicsLayer.AbsoluteLeft, endY + graphicsLayer.AbsoluteTop);

        // Lijnvector
        var dx = end.X - start.X;
        var dy = end.Y - start.Y;

        // Lengte van de lijn
        var length = Convert.ToSingle(Math.Sqrt(dx * dx + dy * dy));
        if (length == 0)
            return;

        // Normale vector (loodrecht op de lijnrichting)
        var nx = -dy / length;
        var ny = dx / length;

        // Gemiddelde schaal voor uniforme dikte in beide richtingen
        var halfThicknessx = strokeWidth * 0.5f;
        var halfThicknessy = strokeWidth * 0.5f;

        // Offset vector voor dikte
        var offsetX = nx * halfThicknessx;
        var offsetY = ny * halfThicknessy;

        // Bereken de 4 hoekpunten van de lijn als rechthoek
        var v1 = new RawVector2(start.X + offsetX, start.Y + offsetY).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);
        var v2 = new RawVector2(start.X - offsetX, start.Y - offsetY).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);
        var v3 = new RawVector2(end.X - offsetX, end.Y - offsetY).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);
        var v4 = new RawVector2(end.X + offsetX, end.Y + offsetY).ToClipSpace(graphicsLayer.Width, graphicsLayer.Height);

        // Voeg als 2 driehoeken toe aan FillVerticesList
        graphicsLayer.TriangleVertices.Add(new Vertex { Position = v1, Color = color });
        graphicsLayer.TriangleVertices.Add(new Vertex { Position = v2, Color = color });
        graphicsLayer.TriangleVertices.Add(new Vertex { Position = v3, Color = color });

        graphicsLayer.TriangleVertices.Add(new Vertex { Position = v3, Color = color });
        graphicsLayer.TriangleVertices.Add(new Vertex { Position = v4, Color = color });
        graphicsLayer.TriangleVertices.Add(new Vertex { Position = v1, Color = color });
    }

}
