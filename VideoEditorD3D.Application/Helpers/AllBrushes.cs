using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Application.Helpers
{
    public class AllBrushes
    {
        public RawColor4 HorizontalLines => new RawColor4(1, 1f, 1f, 1f);
        public RawColor4 Text => new RawColor4(1, 1, 1, 1);
        public RawColor4 VerticalLines => new RawColor4(0.2f, 0.2f, 0.2f, 1.0f);

        public RawColor4 SelectedClip => new RawColor4(0.2f, 0.2f, 0.2f, 1.0f);
        public RawColor4 VideoClip => new RawColor4(0.2f, 0.2f, 0.2f, 1.0f);
        public RawColor4 AudioClip => new RawColor4(0.2f, 0.2f, 0.2f, 1.0f);
        public RawColor4 ClipBorder => new RawColor4(0.2f, 0.2f, 0.2f, 1.0f);

        public RawColor4 PositionLine => new RawColor4(0.2f, 0.2f, 0.2f, 1.0f);
    }
}
