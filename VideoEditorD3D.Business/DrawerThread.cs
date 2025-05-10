using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Types;

namespace VideoEditorD3D.Business;

public class DrawerThread : IDrawerThread
{
    public DrawerThread(IApplicationForm applicationForm, Application application)
    {
        ApplicationForm = applicationForm;
        Application = application;
        DrawThread = new Thread(new ThreadStart(DrawKernel));
    }

    private readonly IApplicationForm ApplicationForm;
    private readonly Application Application;
    private readonly Thread DrawThread;

    public void StartThread()
    {
        DrawThread.Start();
    }

    private void DrawKernel()
    {
        while (!Application.KillSwitch)
        {
            ApplicationForm.Timers.FpsTimer.SleepTillNextFrame(new Fps(1, 60));
            ApplicationForm.Draw();
        }
    }

    public void Dispose()
    {
        Application.KillSwitch = true;
        if (DrawThread != null && DrawThread != Thread.CurrentThread && DrawThread.ThreadState == ThreadState.Running)
        {
            DrawThread.Join();
        }
        GC.SuppressFinalize(this);
    }
}