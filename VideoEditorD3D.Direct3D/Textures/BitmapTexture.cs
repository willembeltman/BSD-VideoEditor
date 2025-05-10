using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using VideoEditorD3D.Direct3D.Interfaces;
using Device = SharpDX.Direct3D11.Device;

namespace VideoEditorD3D.Direct3D.Textures;

public class BitmapTexture : IDisposableTexture
{
    public BitmapTexture(Device device, Bitmap bitmap)
    {
        Bitmap = bitmap;

        var bmpData = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
            System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        try
        {
            var texDesc = new Texture2DDescription
            {
                Width = bitmap.Width,
                Height = bitmap.Height,
                ArraySize = 1,
                BindFlags = BindFlags.ShaderResource,
                Usage = ResourceUsage.Immutable,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.B8G8R8A8_UNorm,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = new SampleDescription(1, 0)
            };

            var dataBox = new DataBox(bmpData.Scan0, bmpData.Stride, 0);
            Texture = new Texture2D(device, texDesc, [dataBox]);
            TextureView = new ShaderResourceView(device, Texture);
        }
        finally
        {
            bitmap.UnlockBits(bmpData);
        }
    }

    public Bitmap Bitmap { get; }
    public Texture2D Texture { get; }
    public ShaderResourceView TextureView { get; }

    public void Dispose()
    {
        TextureView?.Dispose();
        Texture?.Dispose();
        Bitmap?.Dispose();
        GC.SuppressFinalize(this);
    }
}
