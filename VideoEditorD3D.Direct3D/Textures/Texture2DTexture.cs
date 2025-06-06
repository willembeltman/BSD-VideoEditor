using SharpDX.Direct3D11;
using VideoEditorD3D.Direct3D.Interfaces;
using Device = SharpDX.Direct3D11.Device;

namespace VideoEditorD3D.Direct3D.Textures
{
    public class Texture2DTexture : IDisposableTexture
    {
        public Texture2DTexture(Device device, Texture2D texture2D)
        {
            Texture = texture2D;
            TextureView = new ShaderResourceView(device, Texture);
        }

        public Texture2D Texture { get; }
        public ShaderResourceView TextureView { get; }

        public void Dispose()
        {
            TextureView?.Dispose();
            Texture?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
