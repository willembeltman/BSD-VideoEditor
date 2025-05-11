using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Application.Helpers
{
    public class AllBrushes
    {
        public RawColor4 HorizontalLines => new RawColor4(0.2f, 0.2f, 0.2f, 1.0f);

        public RawColor4? Text { get; internal set; }
    }
}
