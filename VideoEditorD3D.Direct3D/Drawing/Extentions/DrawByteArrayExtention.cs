using SharpDX.Direct3D11;
using VideoEditorD3D.Direct3D.Textures;
using VideoEditorD3D.Direct3D.TexturesWithVerticies;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class DrawByteArrayExtention
{
    public static void DrawByteArray(this GraphicsLayer graphicsLayer, int left, int top, int width, int height, byte[] buffer, int bufferWidth, int bufferHeight)
    {
        var absoluteLeft = left + graphicsLayer.AbsoluteLeft;
        var absoluteTop = top + graphicsLayer.AbsoluteTop;
        var vertices = graphicsLayer.CreateTextureVerticesForRectangle(absoluteLeft, absoluteTop, width, height);
        var verticesBuffer = Buffer.Create(graphicsLayer.Device, BindFlags.VertexBuffer, vertices);
        var texture = new ByteArrayTexture(graphicsLayer.Device, buffer, bufferWidth, bufferHeight);
        var image = new DisposableTextureWithVerticies(vertices, verticesBuffer, texture);
        graphicsLayer.TextureImages.Add(image);
    }
}
