using VideoEditorD3D.Direct3D.Types;
using Buffer = SharpDX.Direct3D11.Buffer;

namespace VideoEditorD3D.Direct3D.Interfaces
{
    public interface ITextureImage
    {
        TextureVertex[] Vertices { get; }
        Buffer VerticesBuffer { get; }
        ITexture Texture { get; }
    }
}