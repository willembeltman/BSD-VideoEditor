using SharpDX.Direct3D11;

namespace VideoEditorD3D.Direct3D.Interfaces
{
    public interface ITexture : IDisposable
    {
        Texture2D Texture { get; }
        ShaderResourceView TextureView { get; }
    }
}