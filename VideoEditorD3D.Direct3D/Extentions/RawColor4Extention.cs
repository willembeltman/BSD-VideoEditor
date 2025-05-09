using SharpDX.Mathematics.Interop;
using System.Drawing;

namespace VideoEditorD3D.Direct3D.Extentions;

public static class RawColor4Extention
{
    public static Color ToSystemDrawingColor(this RawColor4 color)
    {
        return Color.FromArgb(
            Convert.ToInt32(color.A * 255),
            Convert.ToInt32(color.R * 255),
            Convert.ToInt32(color.G * 255),
            Convert.ToInt32(color.B * 255));
    }
}
