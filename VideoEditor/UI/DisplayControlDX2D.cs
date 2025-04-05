using SharpDX;
using SharpDX.Direct2D1;
using Bitmap = SharpDX.Direct2D1.Bitmap;
using PixelFormat = SharpDX.Direct2D1.PixelFormat;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using BitmapInterpolationMode = SharpDX.Direct2D1.BitmapInterpolationMode;
using Format = SharpDX.DXGI.Format;
using SharpDX.Mathematics.Interop;
using VideoEditor.Types;
using VideoEditor.Static;
using System.ComponentModel;

namespace VideoEditor.UI;

public class DisplayControlDX2D : BaseControlDX2D
{
    public DisplayControlDX2D(Engine engine) : base(engine)
    {
        Thread = new Thread(new ThreadStart(Kernel));

        SuspendLayout();

        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.Opaque, true);

        ResumeLayout(false);
    }

    public Thread Thread { get; }

    Timeline Timeline => Engine.Timeline;

    private void Kernel()
    {
        while (Engine.IsRunning)
        {
            if (!BeginAutoResetEvent.WaitOne(100)) continue;
            Draw();
            FpsCounter.Tick();
            DoneAutoResetEvent.Set();
        }
    }
    private void Draw()
    {
        if (RenderTarget == null)
            return;

        lock (this)
        {
            float controlWidth = Width;
            float controlHeight = Height;
            float imageWidth = Timeline.Resolution.Width;
            float imageHeight = Timeline.Resolution.Height;

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
            var resolution = new Resolution(Convert.ToInt32(destWidth), Convert.ToInt32(destHeight));

            if (Bitmap == null || Bitmap.PixelSize.Width != resolution.Width || Bitmap.PixelSize.Height != resolution.Height)
            {
                Bitmap?.Dispose();

                var bitmapProperties = new BitmapProperties(new PixelFormat(Format.R8G8B8A8_UNorm, AlphaMode.Ignore));
                Bitmap = new Bitmap(RenderTarget, new Size2(resolution.Width, resolution.Height), bitmapProperties);
            }

            RenderTarget.BeginDraw();

            var frames = Timeline.CurrentVideoClips
                .OrderBy(a => a.Layer)
                .Select(clip => clip.GetCurrentFrame(resolution))
                .Where(a => a != null)
                .ToArray();

            RenderTarget.Clear(new Color4(0, 0, 0, 1)); // Zwarte achtergrond

            foreach (var frame in frames)
            {
                if (frame == null) continue;

                // Update de bitmap rechtstreeks met de framebuffer
                Bitmap.CopyFromMemory((byte[])frame.Buffer, resolution.Width * 4);

                // Tekenen met GPU-scaling en behoud van aspect ratio
                RenderTarget.DrawBitmap(Bitmap, destRect, 1.0f, BitmapInterpolationMode.Linear);
            }

            RenderTarget.EndDraw();
        }
    }

    public void Begin()
    {
        BeginAutoResetEvent.Set();
    }
    public bool Done()
    {
        return DoneAutoResetEvent.WaitOne(500);
    }


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        Bitmap?.Dispose();
        RenderTarget?.Dispose();
        Factory?.Dispose();
        ImagingFactory?.Dispose();
    }

}