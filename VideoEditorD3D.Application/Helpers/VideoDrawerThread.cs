using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Timers;

namespace VideoEditorD3D.Application.Helpers;

public class VideoDrawerThread(ApplicationState applicationContext, IApplicationForm applicationForm)
    : Default60FpsDrawerThread(applicationForm)
{
    public override double FrameTime => applicationContext?.Timeline.Fps ?? 1/25d;
}