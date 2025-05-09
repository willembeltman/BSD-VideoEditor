using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D.Extentions;

public static class RawColor4Extention
{
    public static System.Drawing.Color ToSystemDrawingColor(this RawColor4 color)
    {
        return System.Drawing.Color.FromArgb(
            Convert.ToInt32(color.A * 255),
            Convert.ToInt32(color.R * 255),
            Convert.ToInt32(color.G * 255),
            Convert.ToInt32(color.B * 255));
    }
    public static SharpDX.Color ToSharpDXColor(this RawColor4 color)
    {
        return new SharpDX.Color(
            Convert.ToInt32(color.A * 255),
            Convert.ToInt32(color.R * 255),
            Convert.ToInt32(color.G * 255),
            Convert.ToInt32(color.B * 255));
    }
}
