using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Vertices;
using VideoEditorD3D.Direct3D.Extentions;
using VideoEditorD3D.Direct3D.Textures;
using VideoEditorD3D.Types;
using SharpDX.Direct3D11;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.TextureImages;
using Device = SharpDX.Direct3D11.Device;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D;

public class GraphicsLayer(IApplicationForm application) : IDisposable
{
    private IApplicationForm Application { get; } = application;

    public List<Vertex> LineVertices { get; } = [];
    public List<Vertex> FillVertices { get; } = [];
    public List<DisposableTextureImage> ImageTextures { get; } = [];
    public List<CachedTextureImage> CharacterTextures { get; } = [];

    private int CanvasWidth => Application.Width;
    private int CanvasHeight => Application.Height;
    private Device Device => Application.Device;
    private CharacterCollection Characters => Application.Characters;

    public Buffer? LineVerticesBuffer { get; set; }
    public Buffer? FillVerticesBuffer { get; set; }

    public void StartDrawing()
    {
        LineVerticesBuffer?.Dispose();
        LineVerticesBuffer = null;
        FillVerticesBuffer?.Dispose();
        FillVerticesBuffer = null;
        
        foreach (var image in ImageTextures)
            image.Dispose();
        foreach (var image in CharacterTextures)
            image.Dispose();

        LineVertices.Clear();
        FillVertices.Clear();
        ImageTextures.Clear();
        CharacterTextures.Clear();
    }
    public void EndDrawing()
    {
        if (LineVertices.Count > 0)
            LineVerticesBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, LineVertices.ToArray());
        if (FillVertices.Count > 0)
            FillVerticesBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, FillVertices.ToArray());
    }
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
    public void DrawLineOnePixelWide(RawVector2 start, RawVector2 end, RawColor4 color)
    {
        LineVertices.Add(new Vertex { Position = start.ToClipSpace(CanvasWidth, CanvasHeight), Color = color });
        LineVertices.Add(new Vertex { Position = end.ToClipSpace(CanvasWidth, CanvasHeight), Color = color });
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
    public void DrawBitmap(float left, float top, float width, float height, Bitmap bitmap)
    {
        var vertices = CreateBitmapVertices(left, top, width, height);
        var verticesBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, vertices);
        var texture = new BitmapTexture(Device, bitmap);
        var image = new DisposableTextureImage(vertices, verticesBuffer, texture);
        ImageTextures.Add(image);
    }
    public void DrawFrame(float left, float top, float width, float height, Frame frame)
    {
        var vertices = CreateBitmapVertices(left, top, width, height);
        var verticesBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, vertices);
        var texture = new FrameTexture(Device!, frame);
        var image = new DisposableTextureImage(vertices, verticesBuffer, texture);
        ImageTextures.Add(image);
    }
    public void DrawText(string text, float left, float top, string font = "Arial", float fontSize = 10f, RawColor4? foreColor = null, RawColor4? backColor = null)
    {
        foreColor ??= new RawColor4(1, 1, 1, 1);
        backColor ??= new RawColor4(0, 0, 0, 0);

        var currentLeft = Convert.ToSingle(Math.Round(left));
        var currentTop = Convert.ToSingle(Math.Round(top));
        var currentBottom = float.MaxValue;

        var currentText = text.Replace("\r", "");
        var rows = currentText.Split('\n');

        foreach (var row in rows)
        {
            foreach (var character in row)
            {
                // Text item aanmaken of ophalen voor het huidige character
                var texture = Characters.GetOrCreate(character, font, fontSize, backColor.Value, foreColor.Value);

                // Berekenen
                var right = currentLeft + texture.Width - 1f;
                var bottom = currentTop + texture.Height - 2f;
                if (currentBottom > bottom)
                    currentBottom = bottom;

                // Vertices laten maken
                var fillVertices = CreateBitmapVertices(currentLeft, currentTop, texture.TextureBitmap.Bitmap.Width, texture.TextureBitmap.Bitmap.Height);
                var fillVerticesBuffer = Buffer.Create(Device, BindFlags.VertexBuffer, fillVertices);

                // En dit in een model stoppen
                var image = new CachedTextureImage(fillVertices, fillVerticesBuffer, texture);
                CharacterTextures.Add(image);

                currentLeft = right;
            }
            currentLeft = left;
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
        foreach (var image in ImageTextures)
            image.Dispose();

        GC.SuppressFinalize(this);
    }

}