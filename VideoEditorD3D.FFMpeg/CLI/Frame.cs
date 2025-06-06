using System.Buffers;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.FFMpeg.CLI;

public class Frame(Resolution resolution, long index, double clipTime) : IDisposable
{
    public Resolution Resolution { get; } = resolution;
    public long Index { get; } = index;
    public double ClipTime { get; } = clipTime;
    public byte[] Buffer { get; } = ArrayPool<byte>.Shared.Rent(resolution.ByteLength);

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(Buffer);
    }
}
