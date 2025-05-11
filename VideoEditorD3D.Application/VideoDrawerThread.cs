using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Application;

public class VideoDrawerThread(ApplicationContext application, IApplicationForm applicationForm) : Default60FpsDrawerThread(applicationForm, application)
{
    private readonly ApplicationContext Application = application;
    public override double FrameTime => Application.Timeline.Fps;    
}