using VideoEditor.Static;
using SharpDX.Direct2D1;
using SharpDX.WIC;
using Factory = SharpDX.Direct2D1.Factory;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using SharpDX;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Format = SharpDX.DXGI.Format;
using VideoEditor.Helpers;

namespace VideoEditor.UI
{

    public class BaseControlDX2D : Control
    {
        protected WindowsScaling? Scaling;
        protected Factory? Factory;
        protected WindowRenderTarget? RenderTarget;
        protected Bitmap? Bitmap;
        protected ImagingFactory? ImagingFactory;
        protected SolidColorBrushes? Brushes;

        public BaseControlDX2D(Engine engine)
        {
            Engine = engine;

            FpsCounter = new FpsCounter();
            BeginAutoResetEvent = new AutoResetEvent(false);
            DoneAutoResetEvent = new AutoResetEvent(false);
        }
        internal Engine Engine { get; }
        public FpsCounter FpsCounter { get; }
        public AutoResetEvent BeginAutoResetEvent { get; }
        public AutoResetEvent DoneAutoResetEvent { get; }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            Scaling = new WindowsScaling(Handle);
            Factory = new Factory();
            ImagingFactory = new ImagingFactory();

            int PhysicalWidth = (int)(Width * Scaling);
            int PhysicalHeight = (int)(Height * Scaling);

            var renderTargetProperties = new RenderTargetProperties
            {
                PixelFormat = new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Ignore)
            };

            var hwndProperties = new HwndRenderTargetProperties
            {
                Hwnd = Handle,
                PixelSize = new Size2(PhysicalWidth, PhysicalHeight),
                PresentOptions = PresentOptions.Immediately
            };

            RenderTarget = new WindowRenderTarget(Factory, renderTargetProperties, hwndProperties);
            Brushes = new SolidColorBrushes(RenderTarget);
        }
        protected override void OnResize(EventArgs e)
        {
            lock (this)
            {
                base.OnResize(e);
                if (Scaling == null) return;
                int PhysicalWidth = (int)(Width * Scaling);
                int PhysicalHeight = (int)(Height * Scaling);
                RenderTarget?.Resize(new Size2(PhysicalWidth, PhysicalHeight));
            }
        }
    }
}
