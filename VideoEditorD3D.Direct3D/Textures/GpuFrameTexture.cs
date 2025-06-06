using SharpDX.Direct3D11;
using VideoEditorD3D.Direct3D.Interfaces;
using Device = SharpDX.Direct3D11.Device;

namespace VideoEditorD3D.Direct3D.Textures
{
    public class GpuFrameTexture : IDisposableTexture
    {
        public GpuFrameTexture(Device device, Texture2D texture2D)
        {
            Texture = texture2D;
            TextureView = new ShaderResourceView(device, Texture);
        }

        public ShaderResourceView TextureView { get; }
        public Texture2D Texture { get; }

        public void Dispose()
        {
            TextureView?.Dispose();
            Texture?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
