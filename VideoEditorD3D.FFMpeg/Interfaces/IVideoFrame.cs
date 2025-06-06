using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.FFMpeg.Interfaces
{
    public interface IVideoFrame : IDisposable
    {
        Resolution Resolution { get; }
        long Index { get; }
        double ClipTime { get; }
        bool IsKeyFrame { get; }
    }
}
