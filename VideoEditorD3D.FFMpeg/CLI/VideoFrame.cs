using System.Buffers;
using VideoEditorD3D.FFMpeg.Interfaces;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.FFMpeg.CLI;

public class VideoFrame : IVideoFrame
{
    public VideoFrame(Resolution resolution, long index)
    {
        Resolution = resolution;
        Index = index;
        Buffer = ArrayPool<byte>.Shared.Rent(BufferSize);
    }
    public Resolution Resolution { get; }
    public long Index { get; }
    public bool IsKeyFrame { get; } = true;

    public byte[] Buffer { get; }
    public int BufferSize => Resolution.ByteLength;

    public void Dispose()
    {
        ArrayPool<byte>.Shared.Return(Buffer);
    }
}
