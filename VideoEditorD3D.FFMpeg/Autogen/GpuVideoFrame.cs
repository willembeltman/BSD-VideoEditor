using SharpDX.Direct3D11;
using VideoEditorD3D.FFMpeg.Interfaces;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.FFMpeg.Autogen;

public class GpuVideoFrame(Resolution resolution, Texture2D texture, long index, double clipTime, bool isKeyFrame) : IVideoFrame
{
    public Resolution Resolution { get; } = resolution;
    public long Index { get; } = index;
    public double ClipTime { get; } = clipTime;
    public bool IsKeyFrame { get; } = isKeyFrame;

    public Texture2D Texture { get; } = texture;

    public void Dispose()
    {
        Texture.Dispose();
    }
}
