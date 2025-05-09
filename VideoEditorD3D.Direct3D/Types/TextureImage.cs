using VideoEditorD3D.Direct3D.Types;
using VideoEditorD3D.Direct3D.Textures;

namespace VideoEditorD3D.Direct3D.Types;

public struct TextureImage : IDisposable
{
    public TextureImage(
        TextureVertex[] vertices,
        ITexture texture,
        bool isCached)
    {
        Vertices = vertices;
        Texture = texture;
        IsCached = isCached;
    }

    public TextureVertex[] Vertices { get; }
    public ITexture Texture { get; }
    public bool IsCached { get; }

    public void Dispose()
    {
        Texture.Dispose();
        GC.SuppressFinalize(this);
    }
}