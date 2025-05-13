using SharpDX.Direct3D11;
using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.SharpDXExtentions;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Textures;

public class CharacterTexture : ICachedTexture
{
    public CharacterTexture(
        char character, 
        string fontName, 
        float fontSize, 
        FontStyle fontStyle,
        RawColor4 backColor, 
        RawColor4 foreColor,
        Device device)
    {
        Char = character;
        FontName = fontName;
        FontSize = fontSize;
        FontStyle = fontStyle;
        BackColor = backColor;
        ForeColor = foreColor;

        // Bitmap genereren
        var label = character.ToString();

        // Eerst een tijdelijke bitmap en graphics maken om de grootte te meten
        using (var tempBitmap = new Bitmap(1, 1))
        using (var g = Graphics.FromImage(tempBitmap))
        using (var font = new Font(fontName, fontSize, fontStyle))
        {
            // Instellingen voor kwaliteit
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Meten
            var size = g.MeasureString(label, font);
            MeasureWidth = size.Width;
            MeasureHeight = size.Height;
        }

        // Afmetingen afronden naar hele pixels (gewoon naar beneden want ze zijn altijd veelste groot)
        Width = (int)Math.Floor(MeasureWidth);
        Height = (int)Math.Floor(MeasureHeight);

        // Dan de bitmap tekenen
        using (var bitmap = new Bitmap(Width, Height))
        {
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
                var rect = new RectangleF(0, 0, MeasureWidth, MeasureHeight);

                // Tekst tekenen
                g.DrawString(label, font, brush, rect);
            }

            // Dan de bitmap converteren naar een texture
            BitmapTexture = new BitmapTexture(device, bitmap);
        }
    }

    public char Char { get; }
    public string FontName { get; }
    public float FontSize { get; }
    public FontStyle FontStyle { get; }
    public RawColor4 BackColor { get; }
    public RawColor4 ForeColor { get; }
    public float MeasureWidth { get; }
    public float MeasureHeight { get; }
    public int Width { get; }
    public int Height { get; }
    public BitmapTexture BitmapTexture { get; }
    public Texture2D Texture => BitmapTexture.Texture;
    public ShaderResourceView TextureView => BitmapTexture.TextureView;

    public void Dispose()
    {
        BitmapTexture?.Dispose();
        GC.SuppressFinalize(this);
    }
}
