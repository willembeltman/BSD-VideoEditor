namespace VideoEditorD3D.Direct3D.Extentions
{
    public static class ColorExtention
    {
        public static System.Drawing.Color ToSystemDrawingColor(this SharpDX.Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
