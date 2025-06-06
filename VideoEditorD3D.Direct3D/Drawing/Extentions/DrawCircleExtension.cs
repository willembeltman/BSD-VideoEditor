using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class DrawCircleExtension
{
    private const int DefaultSegments = 36; // Hoe hoger, hoe ronder

    public static void DrawCircle(this GraphicsLayer graphicsLayer,
                                  int centerX, int centerY,
                                  int radius,
                                  RawColor4 color,
                                  int strokeWidth,
                                  int segments = DefaultSegments)
    {
        if (strokeWidth < 1 || radius <= 0 || segments < 3)
            return;

        float angleStep = (float)(2 * Math.PI / segments);
        int prevX = centerX + radius;
        int prevY = centerY;

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep;
            int nextX = centerX + (int)(radius * Math.Cos(angle));
            int nextY = centerY + (int)(radius * Math.Sin(angle));

            graphicsLayer.DrawLine(prevX, prevY, nextX, nextY, color, strokeWidth);

            prevX = nextX;
            prevY = nextY;
        }
    }
}



