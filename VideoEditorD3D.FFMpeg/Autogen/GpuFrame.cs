using SharpDX.Direct3D11;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.FFMpeg.Autogen;

public class GpuFrame(Resolution resolution, Texture2D texture, long frameIndex, double clipTime, bool isKeyFrame) : IDisposable
{
    public Resolution Resolution { get; } = resolution;
    public long FrameIndex { get; } = frameIndex;
    public double ClipTime { get; } = clipTime;
    public Texture2D Texture { get; } = texture;
    public bool IsKeyFrame { get; } = isKeyFrame;

    public void Dispose()
    {
        Texture.Dispose();
    }
}
