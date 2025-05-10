using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Extentions;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Textures;

public class CharacterTexture : ICachedTexture
{
    public CharacterTexture(char character, string fontName, float fontSize, FontStyle fontStyle, RawColor4 backColor, RawColor4 foreColor, Device device)
    {
        Char = character;
        FontName = fontName;
        FontSize = fontSize;
        FontStyle = fontStyle;

        // Bitmap genereren
        var label = character.ToString();

        // Eerst een tijdelijke bitmap en graphics maken om de grootte te meten
        using (var tempBitmap = new Bitmap(100, 100))
        using (var g = Graphics.FromImage(tempBitmap))
        using (var font = new Font(fontName, fontSize, fontStyle))
        {
            // Instellingen voor kwaliteit
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

            // Meten
            var size = g.MeasureString(label, font);
            Width = size.Width;
            Height = size.Height;
        }

        // Afmetingen afronden naar hele pixels
        var bitmapWidth = (int)Math.Ceiling(Width);
        var bitmapHeight = (int)Math.Ceiling(Height);

        var bitmap = new Bitmap(bitmapWidth, bitmapHeight);
        using (var g = Graphics.FromImage(bitmap))
        {
            // Zorg voor transparante achtergrond
            g.Clear(backColor.ToSystemDrawingColor());

            // Instellingen voor kwaliteit
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

            // Font en brush
            using var font = new Font(fontName, fontSize, fontStyle);
            using var brush = new SolidBrush(foreColor.ToSystemDrawingColor());

            // Rect bepalen
            var rect = new RectangleF(0, 0, bitmapWidth, bitmapHeight);

            // Tekst tekenen
            g.DrawString(label, font, brush, rect);
        }

        TextureBitmap = new BitmapTexture(device, bitmap);
    }

    public char Char { get; }
    public string FontName { get; }
    public float FontSize { get; }
    public FontStyle FontStyle { get; }
    public float Width { get; }
    public float Height { get; }
    public int BitmapWidth => TextureBitmap.Bitmap.Width;
    public int BitmapHeight => TextureBitmap.Bitmap.Height;
    public BitmapTexture TextureBitmap { get; }
    public Texture2D Texture => TextureBitmap.Texture;
    public ShaderResourceView TextureView => TextureBitmap.TextureView;


    public void Dispose()
    {
        TextureView?.Dispose();
        Texture?.Dispose();
        GC.SuppressFinalize(this);
    }
}
