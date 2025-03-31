using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.WIC;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using Factory = SharpDX.Direct2D1.Factory;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using BitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;
using Format = SharpDX.DXGI.Format;
using SharpDX.Mathematics.Interop;
using VideoEditor.Types;

namespace VideoEditor.UI;

public class DisplayControlDX2D : Control
{
    private Factory? _factory;
    private WindowRenderTarget? _renderTarget;
    private Bitmap? _bitmap;
    private ImagingFactory? _wicFactory;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern int GetDpiForWindow(IntPtr hwnd);
    private float WindowScaling => GetDpiForWindow(Handle) / 96.0f;
    private int PhysicalWidth => (int)(Width * WindowScaling);
    private int PhysicalHeight => (int)(Height * WindowScaling);

    public DisplayControlDX2D()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);
    }

    public void SetFrame(Frame frame)
    {
        if (_bitmap == null || _bitmap.PixelSize.Width != frame.Resolution.Width || _bitmap.PixelSize.Height != frame.Resolution.Height)
        {
            _bitmap?.Dispose();

            var bitmapProperties = new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Ignore));
            _bitmap = new Bitmap(_renderTarget, new Size2(frame.Resolution.Width, frame.Resolution.Height), bitmapProperties);
        }

        // Update de bitmap rechtstreeks met de framebuffer
        _bitmap.CopyFromMemory(frame.Buffer, frame.Resolution.Width * 4);

        Draw();
    }

    private void Draw()
    {
        if (_renderTarget == null || _bitmap == null)
            return;

        _renderTarget.BeginDraw();
        _renderTarget.Clear(new Color4(0, 0, 0, 1)); // Zwarte achtergrond

        float controlWidth = Width;
        float controlHeight = Height;
        float imageWidth = _bitmap.PixelSize.Width;
        float imageHeight = _bitmap.PixelSize.Height;

        // Bereken de aspect ratio van de afbeelding
        float imageAspect = imageWidth / imageHeight;
        float controlAspect = controlWidth / controlHeight;

        float destWidth, destHeight;
        float offsetX, offsetY;

        if (imageAspect > controlAspect)
        {
            // Beeld is breder dan de control -> Pas hoogte aan
            destWidth = controlWidth;
            destHeight = controlWidth / imageAspect;
            offsetX = 0;
            offsetY = (controlHeight - destHeight) / 2; // Centreer verticaal
        }
        else
        {
            // Beeld is hoger dan de control -> Pas breedte aan
            destHeight = controlHeight;
            destWidth = controlHeight * imageAspect;
            offsetX = (controlWidth - destWidth) / 2; // Centreer horizontaal
            offsetY = 0;
        }

        // Maak een rectangle met correcte scaling en centrering
        var destRect = new RawRectangleF(offsetX, offsetY, offsetX + destWidth, offsetY + destHeight);

        // Tekenen met GPU-scaling en behoud van aspect ratio
        _renderTarget.DrawBitmap(_bitmap, destRect, 1.0f, BitmapInterpolationMode.Linear);

        _renderTarget.EndDraw();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        _factory = new Factory();
        _wicFactory = new ImagingFactory();

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

        _renderTarget = new WindowRenderTarget(_factory, renderTargetProperties, hwndProperties);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Draw();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        _renderTarget?.Resize(new Size2(PhysicalWidth, PhysicalHeight));
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _bitmap?.Dispose();
        _renderTarget?.Dispose();
        _factory?.Dispose();
        _wicFactory?.Dispose();
    }
}