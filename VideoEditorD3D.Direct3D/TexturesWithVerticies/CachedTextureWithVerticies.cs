using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Vertices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D.TexturesWithVerticies;

public readonly struct CachedTextureWithVerticies(TextureVertex[] vertices, Buffer verticesBuffer, ICachedTexture texture) : ITextureImage
{
    public TextureVertex[] Vertices { get; } = vertices;
    public Buffer VerticesBuffer { get; } = verticesBuffer;
    public ICachedTexture Texture { get; } = texture;
    readonly ITexture ITextureImage.Texture => Texture;

    public void Dispose()
    {
        VerticesBuffer.Dispose();
        GC.SuppressFinalize(this);
    }
}