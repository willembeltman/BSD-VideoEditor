using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoEditorD3D.Direct3D.Drawing.Extentions;

public static class DrawTriangleExtension
{
    public static void DrawTriangle(this GraphicsLayer graphicsLayer,
                                    int x1, int y1,
                                    int x2, int y2,
                                    int x3, int y3,
                                    RawColor4 color,
                                    int strokeWidth)
    {
        if (strokeWidth < 1)
            return;

        graphicsLayer.DrawLine(x1, y1, x2, y2, color, strokeWidth);
        graphicsLayer.DrawLine(x2, y2, x3, y3, color, strokeWidth);
        graphicsLayer.DrawLine(x3, y3, x1, y1, color, strokeWidth);
    }
}
