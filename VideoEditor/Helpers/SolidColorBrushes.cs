using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

namespace VideoEditor.Helpers
{
    public class SolidColorBrushes : IDisposable
    {
        public SolidColorBrushes(WindowRenderTarget RenderTarget)
        {
            White = new SolidColorBrush(RenderTarget, new RawColor4(1.0f, 1.0f, 1.0f, 1.0f));
            Red = new SolidColorBrush(RenderTarget, new RawColor4(1.0f, 0.0f, 0.0f, 1.0f));
            Green = new SolidColorBrush(RenderTarget, new RawColor4(0.0f, 1.0f, 0.0f, 1.0f));
            Blue = new SolidColorBrush(RenderTarget, new RawColor4(0.0f, 0.0f, 1.0f, 1.0f));
            Gray = new SolidColorBrush(RenderTarget, new RawColor4(0.5f, 0.5f, 0.5f, 1.0f));
            Black = new SolidColorBrush(RenderTarget, new RawColor4(0.0f, 0.0f, 0.0f, 1.0f));

            VerticalLines = new SolidColorBrush(RenderTarget, new RawColor4(0, 0, 0.6f, 1.0f));
            HorizontalLines = new SolidColorBrush(RenderTarget, new RawColor4(0.4f, 0, 0, 1.0f));
            PositionLine = new SolidColorBrush(RenderTarget, new RawColor4(1.0f, 1.0f, 1.0f, 1.0f));
            VideoClip = new SolidColorBrush(RenderTarget, new RawColor4(0.0f, 0.0f, 0.5f, 1.0f));
            AudioClip = new SolidColorBrush(RenderTarget, new RawColor4(0.1f, 0.1f, 0.7f, 1.0f));
            SelectedClip = new SolidColorBrush(RenderTarget, new RawColor4(1.0f, 0.0f, 0.0f, 1.0f));
            ClipBorder = new SolidColorBrush(RenderTarget, new RawColor4(1.0f, 1.0f, 1.0f, 1.0f));
            Text = new SolidColorBrush(RenderTarget, new RawColor4(1.0f, 1.0f, 1.0f, 1.0f));
        }

        public SolidColorBrush White { get; }
        public SolidColorBrush Red { get; }
        public SolidColorBrush Green { get; }
        public SolidColorBrush Blue { get; }
        public SolidColorBrush Gray { get; }
        public SolidColorBrush Black { get; }
        public SolidColorBrush VerticalLines { get; }
        public SolidColorBrush HorizontalLines { get; }
        public SolidColorBrush PositionLine { get; }
        public SolidColorBrush VideoClip { get; }
        public SolidColorBrush AudioClip { get; }
        public SolidColorBrush SelectedClip { get; }
        public SolidColorBrush ClipBorder { get; }
        public SolidColorBrush Text { get; }

        public void Dispose()
        {
            White.Dispose();
            Red.Dispose();
            Green.Dispose();
            Blue.Dispose();
            Gray.Dispose();
            Black.Dispose();
            VerticalLines.Dispose();
            HorizontalLines.Dispose();
            PositionLine.Dispose();
            VideoClip.Dispose();
            AudioClip.Dispose();
            SelectedClip.Dispose();
            ClipBorder.Dispose();
        }
    }
}
