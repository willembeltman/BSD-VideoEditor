using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Application;

// Easy for quick development now, will change to settings when first design is done
public static class ApplicationConstants
{
    public static int Margin => 10;

    public static RawColor4 HorizontalLines => new RawColor4(0.4f, 0.4f, 0.4f, 1f); // lichtgrijs
    public static RawColor4 VerticalLines => new RawColor4(0.25f, 0.25f, 0.25f, 1.0f); // medium grijs
    public static RawColor4 Text => new RawColor4(1f, 1f, 1f, 1f); // wit

    public static RawColor4 SelectedClip => new RawColor4(0.8f, 0.2f, 0.2f, 1.0f); // roodaccent voor selectie
    public static RawColor4 VideoClip => new RawColor4(0.2f, 0.4f, 0.8f, 1.0f); // blauw
    public static RawColor4 AudioClip => new RawColor4(0.2f, 0.8f, 0.4f, 1.0f); // groenachtig
    public static RawColor4 ClipBorder => new RawColor4(0.6f, 0.6f, 0.6f, 1.0f); // lichtgrijs border

    public static RawColor4 PositionLine => new RawColor4(1f, 1f, 0.2f, 1.0f); // geel
}
