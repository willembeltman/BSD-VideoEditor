using System.Buffers;

namespace VideoEditorD3D.FFMpeg.CLI;

public class AudioFrame(int channels, long index, double clipTime) : IDisposable
{
    public int Channels { get; } = channels;
    public long Index { get; } = index;
    public double ClipTime { get; } = clipTime;
    public byte[] Buffer { get; } = ArrayPool<byte>.Shared.Rent(channels * 2);
    public int BufferSize => channels * 2;

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(Buffer);
    }
}
