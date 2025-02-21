using Emgu.CV;
using Emgu.CV.CvEnum;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System.Runtime.InteropServices;

namespace MyVideoEditor.Controls
{

    public class Direct3DVideoDisplay : UserControl, IDisposable
    {
        private SwapChain swapChain;
        private SharpDX.Direct3D11.Device device;
        private Texture2D backBuffer;
        private RenderTargetView renderView;

        public Direct3DVideoDisplay()
        {
            InitializeDirect3D();
        }

        private void InitializeDirect3D()
        {
            // Create the swap chain description
            var desc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(Width, Height, new Rational(60, 1), SharpDX.DXGI.Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = SharpDX.DXGI.Usage.RenderTargetOutput
            };

            // Create the Direct3D device and swap chain
            SharpDX.Direct3D11.Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, desc, out device, out swapChain);

            // Get the back buffer
            backBuffer = Texture2D.FromSwapChain<Texture2D>(swapChain, 0);
            renderView = new RenderTargetView(device, backBuffer);
        }
        public byte[] ResizeImage(byte[] imageData, int width, int height)
        {
            // Convert byte array to Mat (EmguCV's image container)
            Mat image = new Mat(height, width, DepthType.Cv8U, 3);
            image.SetTo(imageData);

            // Resize the image
            Mat resizedImage = new Mat();
            CvInvoke.Resize(image, resizedImage, new Size(width, height), 0, 0, Inter.Linear);

            // Convert the resized image back to byte array
            byte[] resizedImageData = new byte[resizedImage.Rows * resizedImage.Cols * resizedImage.ElementSize];
            Marshal.Copy(resizedImage.DataPointer, resizedImageData, 0, resizedImageData.Length);

            return resizedImageData;
        }

        public void SetImageData(byte[] bgrbytes2)
        {
            var bgrbytes = ResizeImage(bgrbytes2, Width, Height);

            // Update the back buffer with the new frame data
            DataStream dataStream;
            device.ImmediateContext.MapSubresource(backBuffer, 0, MapMode.WriteDiscard, SharpDX.Direct3D11.MapFlags.None, out dataStream);
            dataStream.WriteRange(bgrbytes);
            device.ImmediateContext.UnmapSubresource(backBuffer, 0);

            // Present the rendered frame
            swapChain.Present(1, PresentFlags.None);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                // Release DirectX resources
                renderView.Dispose();
                backBuffer.Dispose();
                swapChain.Dispose();
                device.Dispose();
            }
        }
    }
}
