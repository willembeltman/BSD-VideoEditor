using System.Buffers;

namespace VideoEditorD3D.FFMpeg.Types;

public class Frame : IDisposable
{
    public Frame(Resolution resolution, long index)
    {
        Resolution = resolution;
        Index = index;
        Buffer = ArrayPool<byte>.Shared.Rent(resolution.ByteLength);
    }

    public Resolution Resolution { get; }
    public long Index { get; set; }
    public byte[] Buffer { get; }

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(Buffer);
    }
}
