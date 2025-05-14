using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Timers;

namespace VideoEditorD3D.Application.Helpers;

public class VideoDrawerThread(ApplicationContext application, IApplicationForm applicationForm) 
    : Default60FpsDrawerThread(applicationForm, application)
{
    public override double FrameTime => application.Timeline.Fps;    
}