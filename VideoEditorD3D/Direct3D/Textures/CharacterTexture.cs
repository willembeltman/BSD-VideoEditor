using SharpDX.Direct3D11;

namespace VideoEditorD3D.Direct3D.Textures
{
    public class CharacterTexture : ITexture
    {
        public CharacterTexture(char character, string fontName, float fontSize, Color backColor, Color foreColor, Device device)
        {
            Char = character;
            FontName = fontName;
            FontSize = fontSize;

            // Bitmap genereren
            var label = character.ToString();

            // Eerst een tijdelijke bitmap en graphics maken om de grootte te meten
            using (var tempBitmap = new Bitmap(1, 1))
            using (var g = Graphics.FromImage(tempBitmap))
            using (var font = new Font(fontName, fontSize, FontStyle.Regular))
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
            int width = (int)Math.Ceiling(Width);
            int height = (int)Math.Ceiling(Height);

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                // Zorg voor transparante achtergrond
                g.Clear(backColor);

                // Instellingen voor kwaliteit
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

                // Font en brush
                using (var font = new Font(fontName, fontSize, FontStyle.Regular))
                using (var brush = new SolidBrush(foreColor))
                {
                    // Rect bepalen
                    var rect = new RectangleF(0, 0, width, height);

                    // Tekst tekenen
                    g.DrawString(label, font, brush, rect);
                }
            }

            TextureBitmap = new BitmapTexture(device, bitmap);
        }

        public char Char { get; }
        public string FontName { get; }
        public float FontSize { get; }
        public float Width { get; }
        public float Height { get; }
        public BitmapTexture TextureBitmap { get; }
        public Texture2D Texture => TextureBitmap.Texture;
        public ShaderResourceView TextureView => TextureBitmap.TextureView;

        public void Dispose()
        {
            // Managed through CharacterCollection (otherwise we are disposing the cache
        }
    }
}
