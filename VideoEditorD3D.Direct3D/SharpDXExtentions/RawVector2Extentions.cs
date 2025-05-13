using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D.SharpDXExtentions;

public static class RawVector2Extentions
{
    public static RawVector2 ToClipSpace(this RawVector2 point, int PhysicalWidth, int PhysicalHeight)
    {
        float clipX = (point.X / (PhysicalWidth / 2f)) - 1f;
        float clipY = 1f - (point.Y / (PhysicalHeight / 2f));
        return new RawVector2(clipX, clipY);
    }
}


