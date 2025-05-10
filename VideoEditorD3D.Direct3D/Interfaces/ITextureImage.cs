using VideoEditorD3D.Direct3D.Vertices;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D.Interfaces;

public interface ITextureImage : IDisposable
{
    TextureVertex[] Vertices { get; }
    Buffer VerticesBuffer { get; }
    ITexture Texture { get; }
}