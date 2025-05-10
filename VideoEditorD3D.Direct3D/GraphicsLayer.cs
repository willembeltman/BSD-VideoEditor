using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Vertices;
using VideoEditorD3D.Direct3D.Extentions;
using VideoEditorD3D.Direct3D.Textures;
using SharpDX.Direct3D11;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.TextureImages;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D;

public class GraphicsLayer(IApplicationForm applicationForm) : IDisposable
{
    private IApplicationForm ApplicationForm { get; } = applicationForm;

    public List<Vertex> LineVertices { get; } = [];
    public Buffer? LineVerticesBuffer { get; set; }
    public List<Vertex> TriangleVertices { get; } = [];
    public Buffer? TriangleVerticesBuffer { get; set; }
    public List<ITextureImage> TextureImages { get; } = [];

    public void StartDrawing()
    {
        LineVerticesBuffer?.Dispose();
        LineVerticesBuffer = null;
        TriangleVerticesBuffer?.Dispose();
        TriangleVerticesBuffer = null;
        
        foreach (var image in TextureImages)
            image.Dispose();
        foreach (var image in TextureImages)
            image.Dispose();

        LineVertices.Clear();
        TriangleVertices.Clear();
        TextureImages.Clear();
        TextureImages.Clear();
    }
    public void EndDrawing()
    {
        if (LineVertices.Count > 0)
            LineVerticesBuffer = Buffer.Create(ApplicationForm.Device, BindFlags.VertexBuffer, LineVertices.ToArray());
        if (TriangleVertices.Count > 0)
            TriangleVerticesBuffer = Buffer.Create(ApplicationForm.Device, BindFlags.VertexBuffer, TriangleVertices.ToArray());
    }
    public void DrawLine(int startX, int startY, int endX, int endY, RawColor4 color, int strokeWidth)
    {
        if (strokeWidth < 1)
            return;

        RawVector2 start = new RawVector2(startX, startY);
        RawVector2 end = new RawVector2(endX, endY);

        if (strokeWidth == 1)
        {
            DrawLineOnePixelWide(startX, startY, endX, endY, color);
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
        var halfThicknessx = strokeWidth * 0.5f;
        var halfThicknessy = strokeWidth * 0.5f;

        // Offset vector voor dikte
        var offsetX = nx * halfThicknessx;
        var offsetY = ny * halfThicknessy;

        // Bereken de 4 hoekpunten van de lijn als rechthoek
        var v1 = new RawVector2(start.X + offsetX, start.Y + offsetY).ToClipSpace(ApplicationForm.Width, ApplicationForm.Height);
        var v2 = new RawVector2(start.X - offsetX, start.Y - offsetY).ToClipSpace(ApplicationForm.Width, ApplicationForm.Height);
        var v3 = new RawVector2(end.X - offsetX, end.Y - offsetY).ToClipSpace(ApplicationForm.Width, ApplicationForm.Height);
        var v4 = new RawVector2(end.X + offsetX, end.Y + offsetY).ToClipSpace(ApplicationForm.Width, ApplicationForm.Height);

        // Voeg als 2 driehoeken toe aan FillVerticesList
        TriangleVertices.Add(new Vertex { Position = v1, Color = color });
        TriangleVertices.Add(new Vertex { Position = v2, Color = color });
        TriangleVertices.Add(new Vertex { Position = v3, Color = color });

        TriangleVertices.Add(new Vertex { Position = v3, Color = color });
        TriangleVertices.Add(new Vertex { Position = v4, Color = color });
        TriangleVertices.Add(new Vertex { Position = v1, Color = color });
    }
    public void DrawLineOnePixelWide(int startX, int startY, int endX, int endY, RawColor4 color)
    {
        RawVector2 start = new RawVector2(startX, startY);
        RawVector2 end = new RawVector2(endX, endY);
        LineVertices.Add(new Vertex { Position = start.ToClipSpace(ApplicationForm.Width, ApplicationForm.Height), Color = color });
        LineVertices.Add(new Vertex { Position = end.ToClipSpace(ApplicationForm.Width, ApplicationForm.Height), Color = color });
    }
    public void DrawRectangle(int left, int top, int width, int height, RawColor4 color, int strokeWidth)
    {
        if (strokeWidth < 1)
            return;

        var right = left + width;
        var bottom = top + height;

        DrawLine(left, top, right, top, color, strokeWidth);
        DrawLine(right, top, right, bottom, color, strokeWidth);
        DrawLine(right, bottom, left, bottom, color, strokeWidth);
        DrawLine(left, bottom, left, top, color, strokeWidth);
    }
    public void FillRectangle(int left, int top, int width, int height, RawColor4 color)
    {
        var right = left + width;
        var bottom = top + height;

        // Clip space coordinaten
        var p1 = new RawVector2(left, top).ToClipSpace(ApplicationForm.Width, ApplicationForm.Height);
        var p2 = new RawVector2(right, top).ToClipSpace(ApplicationForm.Width, ApplicationForm.Height);
        var p3 = new RawVector2(right, bottom).ToClipSpace(ApplicationForm.Width, ApplicationForm.Height);
        var p4 = new RawVector2(left, bottom).ToClipSpace(ApplicationForm.Width, ApplicationForm.Height);

        // Vulling met 2 driehoeken
        if (color.A > 0)
        {
            TriangleVertices.Add(new Vertex { Position = p1, Color = color });
            TriangleVertices.Add(new Vertex { Position = p2, Color = color });
            TriangleVertices.Add(new Vertex { Position = p3, Color = color });

            TriangleVertices.Add(new Vertex { Position = p3, Color = color });
            TriangleVertices.Add(new Vertex { Position = p4, Color = color });
            TriangleVertices.Add(new Vertex { Position = p1, Color = color });
        }
    }
    /// <summary>
    /// Tekent een bitmap in de opgegeven rechthoek. Let op: De bitmap wordt niet automatisch vrijgegeven,
    /// dus zorg ervoor dat je deze zelf dispose't.
    /// </summary>
    public void DrawBitmap(int left, int top, int width, int height, Bitmap bitmap)
    {
        var vertices = CreateTextureVertices(left, top, width, height);
        var verticesBuffer = Buffer.Create(ApplicationForm.Device, BindFlags.VertexBuffer, vertices);
        var texture = new BitmapTexture(ApplicationForm.Device, bitmap);
        var image = new DisposableTextureImage(vertices, verticesBuffer, texture);
        TextureImages.Add(image);
    }
    public void DrawFrame(int left, int top, int width, int height, IFrame frame)
    {
        var vertices = CreateTextureVertices(left, top, width, height);
        var verticesBuffer = Buffer.Create(ApplicationForm.Device, BindFlags.VertexBuffer, vertices);
        var texture = new FrameTexture(ApplicationForm.Device, frame);
        var image = new DisposableTextureImage(vertices, verticesBuffer, texture);
        TextureImages.Add(image);
    }
    public void DrawText(string text, int left, int top, int width = -1, int height = -1, string font = "Ebrima", float fontSize = 10f, FontStyle fontStyle = FontStyle.Regular, int letterSpacing = -2, RawColor4? foreColor = null, RawColor4? backColor = null)
    {
        foreColor ??= new RawColor4(1, 1, 1, 1);
        backColor ??= new RawColor4(0, 0, 0, 0);

        var currentLeft = left;
        var currentTop = top;
        var currentBottom = 0;

        var currentText = text.Replace("\r", "");
        var rows = currentText.Split('\n');

        foreach (var row in rows)
        {
            foreach (var character in row)
            {
                // Text item aanmaken of ophalen voor het huidige character
                var texture = ApplicationForm.Characters.GetOrCreate(character, font, fontSize, fontStyle, backColor.Value, foreColor.Value);

                // Berekenen
                var right = currentLeft + texture.Width + letterSpacing;
                if (width != -1 && height != -1 && right > left + width)
                {
                    // Nieuwe regel
                    currentLeft = left;
                    currentTop = currentBottom;
                    right = currentLeft + texture.Width + letterSpacing;
                }

                var bottom = currentTop + texture.Height;
                if (currentBottom < bottom)
                    currentBottom = bottom;

                // Vertices laten maken
                var fillVertices = CreateTextureVertices(currentLeft, currentTop, texture.Width, texture.Height);
                var fillVerticesBuffer = Buffer.Create(ApplicationForm.Device, BindFlags.VertexBuffer, fillVertices);

                // En dit in een model stoppen
                var image = new CachedTextureImage(fillVertices, fillVerticesBuffer, texture);
                TextureImages.Add(image);

                currentLeft = right;
            }
            // Nieuwe regel
            currentLeft = left;
            currentTop = currentBottom;
        }
    }

    private TextureVertex[] CreateTextureVertices(int left, int top, int width, int height)
    {
        // Afronden op hele pixels
        var roundLeft = Convert.ToSingle(left);
        var roundTop = Convert.ToSingle(top);
        var roundWidth = Convert.ToSingle(width);
        var roundHeight = Convert.ToSingle(height);
        var roundRight = left + roundWidth;
        var roundBottom = top + roundHeight;

        // Maak vertices
        float x0 = roundLeft / ApplicationForm.Width * 2f - 1f;
        float y0 = 1f - roundTop / ApplicationForm.Height * 2f;
        float x1 = roundRight / ApplicationForm.Width * 2f - 1f;
        float y1 = 1f - roundBottom / ApplicationForm.Height * 2f;

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
        foreach (var image in TextureImages)
            image.Dispose();

        GC.SuppressFinalize(this);
    }
}