using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Vertices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D.TexturesWithVerticies;

public readonly struct DisposableTextureWithVerticies(TextureVertex[] vertices, Buffer verticesBuffer, IDisposableTexture texture) : ITextureWithVerticies
{
    public TextureVertex[] Vertices { get; } = vertices;
    public Buffer VerticesBuffer { get; } = verticesBuffer;
    public IDisposableTexture Texture { get; } = texture;
    readonly ITexture ITextureWithVerticies.Texture => Texture;

    public void Dispose()
    {
        VerticesBuffer.Dispose();
        Texture.Dispose();
        GC.SuppressFinalize(this);
    }
}
