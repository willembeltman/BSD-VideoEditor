using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using VideoEditorD3D.Direct3D.Interfaces;
using Device = SharpDX.Direct3D11.Device;

namespace VideoEditorD3D.Direct3D.Textures;

public class ByteArrayTexture : IDisposableTexture
{
    public ByteArrayTexture(Device device, byte[] frameBuffer, int frameWidth, int frameHeight)
    {
        var texDesc = new Texture2DDescription
        {
            Width = frameWidth,
            Height = frameHeight,
            MipLevels = 1,
            ArraySize = 1,
            Format = Format.R8G8B8A8_UNorm,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Immutable,
            BindFlags = BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        };

        var _handle = GCHandle.Alloc(frameBuffer, GCHandleType.Pinned);

        try
        {
            var pointer = _handle.AddrOfPinnedObject();
            var stride = frameWidth * 4;
            var dataBox = new DataBox(pointer, stride, 0);
            Texture = new Texture2D(device, texDesc, [dataBox]);
            TextureView = new ShaderResourceView(device, Texture);
        }
        finally
        {
            if (_handle.IsAllocated)
                _handle.Free();
        }
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