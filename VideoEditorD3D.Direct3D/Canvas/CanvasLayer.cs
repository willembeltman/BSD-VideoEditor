using SharpDX;
using SharpDX.Mathematics.Interop;
using VideoeditorD3D.Direct3D.Types;
using VideoEditorD3D.Direct3D.Extentions;
using VideoEditorD3D.Direct3D.Textures;
using VideoEditorD3D.Direct3D.Types;
using VideoEditorD3D.Types;
using Device = SharpDX.Direct3D11.Device;

namespace VideoEditorD3D.Direct3D.Canvas
{
    public class CanvasLayer : IDisposable
    {
        public CanvasLayer(
            int index,
            Device device,
            CharacterCollection textCharacters,
            int canvasWidth,
            int canvasHeight)
        {
            Index = index;
            Device = device;
            TextCharacters = textCharacters;
            CanvasWidth = canvasWidth;
            CanvasHeight = canvasHeight;

            LineVertices = new List<Vertex>();
            FillVertices = new List<Vertex>();
            Images = new List<TextureImage>();
            Images = new List<TextureImage>();
        }

        private Device Device;
        private CharacterCollection TextCharacters;
        private int CanvasWidth;
        private int CanvasHeight;

        public int Index { get; }


        public List<Vertex> LineVertices { get; }
        public List<Vertex> FillVertices { get; }
        public List<TextureImage> Images { get; }

        public void DrawLine(RawVector2 start, RawVector2 end, RawColor4 color, float strokeWidth)
        {
            if (strokeWidth > 0.9 && strokeWidth < 1.1)
            {
                DrawLineOnePixelWide(start, end, color);
                return;
            }

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
            float halfThicknessx = strokeWidth * 0.5f;
            float halfThicknessy = strokeWidth * 0.5f;

            // Offset vector voor dikte
            var offsetX = nx * halfThicknessx;
            var offsetY = ny * halfThicknessy;

            // Bereken de 4 hoekpunten van de lijn als rechthoek
            var v1 = new RawVector2(start.X + offsetX, start.Y + offsetY).ToClipSpace(CanvasWidth, CanvasHeight);
            var v2 = new RawVector2(start.X - offsetX, start.Y - offsetY).ToClipSpace(CanvasWidth, CanvasHeight);
            var v3 = new RawVector2(end.X - offsetX, end.Y - offsetY).ToClipSpace(CanvasWidth, CanvasHeight);
            var v4 = new RawVector2(end.X + offsetX, end.Y + offsetY).ToClipSpace(CanvasWidth, CanvasHeight);

            // Voeg als 2 driehoeken toe aan FillVerticesList
            FillVertices.Add(new Vertex { Position = v1, Color = color });
            FillVertices.Add(new Vertex { Position = v2, Color = color });
            FillVertices.Add(new Vertex { Position = v3, Color = color });

            FillVertices.Add(new Vertex { Position = v3, Color = color });
            FillVertices.Add(new Vertex { Position = v4, Color = color });
            FillVertices.Add(new Vertex { Position = v1, Color = color });
        }
        public void DrawLineOnePixelWide(RawVector2 Start, RawVector2 End, RawColor4 Color)
        {
            LineVertices.Add(new Vertex { Position = Start.ToClipSpace(CanvasWidth, CanvasHeight), Color = Color });
            LineVertices.Add(new Vertex { Position = End.ToClipSpace(CanvasWidth, CanvasHeight), Color = Color });
        }
        public void DrawRectangle(float left, float top, float width, float height, RawColor4 color, float strokeWidth)
        {
            float right = left + width;
            float bottom = top + height;


            // Clip space coordinaten
            var p1 = new RawVector2(left, top).ToClipSpace(CanvasWidth, CanvasHeight);
            var p2 = new RawVector2(right, top).ToClipSpace(CanvasWidth, CanvasHeight);
            var p3 = new RawVector2(right, bottom).ToClipSpace(CanvasWidth, CanvasHeight);
            var p4 = new RawVector2(left, bottom).ToClipSpace(CanvasWidth, CanvasHeight);

            // Rand met lijnen
            if (strokeWidth > 0 && color.A > 0)
            {
                LineVertices.Add(new Vertex { Position = p1, Color = color });
                LineVertices.Add(new Vertex { Position = p2, Color = color });

                LineVertices.Add(new Vertex { Position = p2, Color = color });
                LineVertices.Add(new Vertex { Position = p3, Color = color });

                LineVertices.Add(new Vertex { Position = p3, Color = color });
                LineVertices.Add(new Vertex { Position = p4, Color = color });

                LineVertices.Add(new Vertex { Position = p4, Color = color });
                LineVertices.Add(new Vertex { Position = p1, Color = color });
            }
        }
        public void FillRectangle(float left, float top, float width, float height, RawColor4 color)
        {
            float right = left + width;
            float bottom = top + height;

            // Clip space coordinaten
            var p1 = new RawVector2(left, top).ToClipSpace(CanvasWidth, CanvasHeight);
            var p2 = new RawVector2(right, top).ToClipSpace(CanvasWidth, CanvasHeight);
            var p3 = new RawVector2(right, bottom).ToClipSpace(CanvasWidth, CanvasHeight);
            var p4 = new RawVector2(left, bottom).ToClipSpace(CanvasWidth, CanvasHeight);

            // Vulling met 2 driehoeken
            if (color.A > 0)
            {
                FillVertices.Add(new Vertex { Position = p1, Color = color });
                FillVertices.Add(new Vertex { Position = p2, Color = color });
                FillVertices.Add(new Vertex { Position = p3, Color = color });

                FillVertices.Add(new Vertex { Position = p3, Color = color });
                FillVertices.Add(new Vertex { Position = p4, Color = color });
                FillVertices.Add(new Vertex { Position = p1, Color = color });
            }
        }
        public void DrawBitmap(float Left, float Top, float Width, float Height, System.Drawing.Bitmap Bitmap)
        {
            var fillVertices = CreateBitmapVertices(Left, Top, Width, Height);
            var texture = new BitmapTexture(Device!, Bitmap);
            var image = new TextureImage(fillVertices, texture, false);
            Images.Add(image);
        }
        public void DrawFrame(float Left, float Top, float Width, float Height, Frame frame)
        {
            var fillVertices = CreateBitmapVertices(Left, Top, Width, Height);
            var texture = new FrameTexture(Device!, frame);
            var image = new TextureImage(fillVertices, texture, false);
            Images.Add(image);
        }
        public void DrawText(string Text, float Left, float Top, float Width, float Height, string Font = "Arial", float FontSize = 12f, Color? ForeColor = null, Color? BackColor = null)
        {
            ForeColor = ForeColor ?? Color.White;
            BackColor = BackColor ?? Color.Transparent;

            var currentLeft = Convert.ToSingle(Math.Round(Left));
            var currentTop = Convert.ToSingle(Math.Round(Top));
            var currentBottom = float.MaxValue;

            var currentText = Text.Replace("\r", "");
            var rows = currentText.Split('\n');

            foreach (var row in rows)
            {
                foreach (var character in row)
                {
                    // Text item aanmaken of ophalen voor het huidige character
                    var texture = TextCharacters.GetOrCreate(character, Font, FontSize, BackColor.Value, ForeColor.Value);

                    // Berekenen
                    var right = currentLeft + texture.Width - 1f;
                    var bottom = currentTop + texture.Height - 2f;
                    if (currentBottom > bottom)
                        currentBottom = bottom;

                    // Vertices laten maken
                    var vertices = CreateBitmapVertices(currentLeft, currentTop, texture.TextureBitmap.Bitmap.Width, texture.TextureBitmap.Bitmap.Height);

                    // En dit in een model stoppen
                    var image = new TextureImage(vertices, texture, true);
                    Images.Add(image);

                    currentLeft = right;
                }
                currentLeft = Left;
                currentTop = currentBottom;
            }

            //return;

            //var label = text.Text;
            //var width = Convert.ToInt32(text.Width);
            //var height = Convert.ToInt32(text.Height);
            //var fontName = text.Font;
            //var fontSize = text.FontSize;

            //var bitmap = new Bitmap(width, height);

            //using (var g = Graphics.FromImage(bitmap))
            //{
            //    // Zorg voor transparante achtergrond
            //    g.Clear(text.BackColor);

            //    // Instellingen voor kwaliteit
            //    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            //    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

            //    // Font en brush
            //    using (var font = new Font(fontName, fontSize, FontStyle.Regular))
            //    using (var brush = new SolidBrush(text.ForeColor))
            //    {
            //        // Rect bepalen
            //        var rect = new RectangleF(0, 0, width, height);

            //        // Tekst tekenen
            //        g.DrawString(label, font, brush, rect);
            //    }
            //}

            //DrawBitmap(new DrawBitmap()
            //{
            //    Text = text.Text,
            //    Left = text.Left,
            //    Top = text.Top,
            //    Width = text.Width,
            //    Height = text.Height,
            //    Bitmap = bitmap,
            //});
        }

        private TextureVertex[] CreateBitmapVertices(float left, float top, float width, float height)
        {
            // Afronden op hele pixels

            var roundLeft = Convert.ToSingle(Math.Round(left));
            var roundTop = Convert.ToSingle(Math.Round(top));
            var roundWidth = Convert.ToSingle(Math.Ceiling(width));
            var roundHeight = Convert.ToSingle(Math.Ceiling(height));
            var roundRight = left + roundWidth;
            var roundBottom = top + roundHeight;

            // Maak vertices
            float x0 = roundLeft / CanvasWidth * 2f - 1f;
            float y0 = 1f - roundTop / CanvasHeight * 2f;
            float x1 = roundRight / CanvasWidth * 2f - 1f;
            float y1 = 1f - roundBottom / CanvasHeight * 2f;

            var FillVertices = new[]
            {
                new TextureVertex { Position = new RawVector2(x0, y0), UV = new RawVector2(0, 0) },
                new TextureVertex { Position = new RawVector2(x1, y0), UV = new RawVector2(1, 0) },
                new TextureVertex { Position = new RawVector2(x0, y1), UV = new RawVector2(0, 1) },
                new TextureVertex { Position = new RawVector2(x1, y0), UV = new RawVector2(1, 0) },
                new TextureVertex { Position = new RawVector2(x1, y1), UV = new RawVector2(1, 1) },
                new TextureVertex { Position = new RawVector2(x0, y1), UV = new RawVector2(0, 1) },
            };
            return FillVertices;
        }

        public void Dispose()
        {
            foreach (var image in Images)
                image.Dispose();
        }
    }
}