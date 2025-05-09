using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Vertices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D.TextureImages;

public readonly struct DisposableTextureImage(TextureVertex[] vertices, Buffer verticesBuffer, IDisposableTexture texture) : ITextureImage
{
    public TextureVertex[] Vertices { get; } = vertices;
    public Buffer VerticesBuffer { get; } = verticesBuffer;
    public IDisposableTexture Texture { get; } = texture;
    readonly ITexture ITextureImage.Texture => Texture;

    public void Dispose()
    {
        VerticesBuffer.Dispose();
        Texture.Dispose();
        GC.SuppressFinalize(this);
    }
}
