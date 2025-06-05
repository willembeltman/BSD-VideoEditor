using System.Buffers;

namespace VideoEditorD3D.FFMpeg.Types;

public class Frame : IDisposable
{
    public Frame(Resolution resolution, long index, double clipTime)
    {
        Resolution = resolution;
        Index = index;
        ClipTime = clipTime;
        Buffer = ArrayPool<byte>.Shared.Rent(resolution.ByteLength);
    }

    public Resolution Resolution { get; }
    public long Index { get; }
    public double ClipTime { get; }
    public byte[] Buffer { get; }

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(Buffer);
    }
}
