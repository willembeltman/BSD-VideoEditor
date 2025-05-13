using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class DrawRectangleExtention
{
    public static void DrawRectangle(this GraphicsLayer graphicsLayer, int left, int top, int width, int height, RawColor4 color, int strokeWidth)
    {
        if (strokeWidth < 1)
            return;

        var right = left + width;
        var bottom = top + height;

        graphicsLayer.DrawLine(left, top, right, top, color, strokeWidth);
        graphicsLayer.DrawLine(right, top, right, bottom, color, strokeWidth);
        graphicsLayer.DrawLine(right, bottom, left, bottom, color, strokeWidth);
        graphicsLayer.DrawLine(left, bottom, left, top, color, strokeWidth);
    }
}
