using SharpDX.Direct3D11;

namespace VideoEditorD3D.Direct3D.Textures
{
    public interface ITexture : IDisposable
    {
        Texture2D Texture { get; }
        ShaderResourceView TextureView { get; }
    }
}