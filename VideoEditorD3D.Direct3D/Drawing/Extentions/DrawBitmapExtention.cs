using SharpDX.Direct3D11;
using VideoEditorD3D.Direct3D.Textures;
using VideoEditorD3D.Direct3D.TexturesWithVerticies;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class DrawBitmapExtention
{
    /// <summary>
    /// Tekent een bitmap in de opgegeven rechthoek. Let op: De bitmap wordt niet automatisch vrijgegeven,
    /// dus zorg ervoor dat je deze zelf dispose't.
    /// </summary>
    public static void DrawBitmap(this GraphicsLayer graphicsLayer, int left, int top, int width, int height, Bitmap bitmap)
    {
        var absoluteLeft = left + graphicsLayer.AbsoluteLeft;
        var absoluteTop = top + graphicsLayer.AbsoluteTop;
        var vertices = graphicsLayer.CreateTextureVerticesForRectangle(absoluteLeft, absoluteTop, width, height);
        var verticesBuffer = Buffer.Create(graphicsLayer.Device, BindFlags.VertexBuffer, vertices);
        var texture = new BitmapTexture(graphicsLayer.Device, bitmap);
        var image = new DisposableTextureWithVerticies(vertices, verticesBuffer, texture);
        graphicsLayer.TextureImages.Add(image);
    }
}
