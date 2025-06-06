using VideoEditorD3D.FFMpeg.Interfaces;

namespace VideoEditorD3D.Application.Buffers
{
    public interface IVideoBuffer : IDisposable
    {
        double TimelineEndTime { get; }
        double TimelineStartTime { get; }
        int TimelineLayer { get; }

        void StartThread();
        IVideoFrame GetCurrentFrame();
    }
}