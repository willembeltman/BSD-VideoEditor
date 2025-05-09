using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using VideoEditorD3D.Direct3D.Helpers;
using VideoEditorD3D.Types;
using Device = SharpDX.Direct3D11.Device;

namespace VideoEditorD3D.Direct3D.Textures;

public class FrameTexture : ITexture
{
    public FrameTexture(Device device, Frame frame)
    {
        var texDesc = new Texture2DDescription
        {
            Width = frame.Width,
            Height = frame.Height,
            MipLevels = 1,
            ArraySize = 1,
            Format = Format.B8G8R8A8_UNorm,
            SampleDescription = new SampleDescription(1, 0),
            Usage = ResourceUsage.Immutable,
            BindFlags = BindFlags.ShaderResource,
            CpuAccessFlags = CpuAccessFlags.None,
            OptionFlags = ResourceOptionFlags.None
        };

        using var reader = new FrameDataPointer(frame);
        var stride = frame.Width * 4;
        var dataBox = new DataBox(reader.DataPointer, stride, 0);
        Texture = new Texture2D(device, texDesc, new[] { dataBox });
        TextureView = new ShaderResourceView(device, Texture);
    }

    public Texture2D Texture { get; }
    public ShaderResourceView TextureView { get; }

    public void Dispose()
    {
        TextureView?.Dispose();
        Texture?.Dispose();
    }
}