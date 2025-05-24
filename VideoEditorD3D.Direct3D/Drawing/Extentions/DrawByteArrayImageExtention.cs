using SharpDX.Direct3D11;
using VideoEditorD3D.Direct3D.Textures;
using VideoEditorD3D.Direct3D.TexturesWithVerticies;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D.Drawing;

public static class DrawByteArrayImageExtention
{
    public static void DrawByteArrayImage(this GraphicsLayer graphicsLayer, int left, int top, int width, int height, byte[] frameBuffer, int frameWidth, int frameHeight)
    {
        var absoluteLeft = left + graphicsLayer.AbsoluteLeft;
        var absoluteTop = top + graphicsLayer.AbsoluteTop;
        var vertices = graphicsLayer.CreateTextureVerticesForRectangle(absoluteLeft, absoluteTop, width, height);
        var verticesBuffer = Buffer.Create(graphicsLayer.Device, BindFlags.VertexBuffer, vertices);
        var texture = new FrameTexture(graphicsLayer.Device, frameBuffer, frameWidth, frameHeight);
        var image = new DisposableTextureWithVerticies(vertices, verticesBuffer, texture);
        graphicsLayer.TextureImages.Add(image);
    }
}
