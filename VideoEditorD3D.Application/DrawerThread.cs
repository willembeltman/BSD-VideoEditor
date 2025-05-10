using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Types;

namespace VideoEditorD3D.Application;

public class DrawerThread : IDrawerThread
{
    public DrawerThread(ApplicationContext application, IApplicationForm applicationForm)
    {
        Application = application;
        ApplicationForm = applicationForm;
        Thread = new Thread(new ThreadStart(Kernel))
        {
            Name = "DrawThread kernel"
        };
    }

    private readonly ApplicationContext Application;
    private readonly IApplicationForm ApplicationForm;
    private readonly Thread Thread;

    public void StartThread()
    {
        Thread.Start();
    }

    private void Kernel()
    {
        while (!Application.KillSwitch)
        {
            ApplicationForm.Timers.FpsTimer.SleepTillNextFrame(new Fps(1, 60));
            ApplicationForm.TryDraw();
        }
        ApplicationForm.CloseForm();
    }

    public void Dispose()
    {
        Application.KillSwitch = true;
        if (Thread != null && Thread != Thread.CurrentThread && Thread.ThreadState == ThreadState.Running)
        {
            Thread.Join();
        }
        GC.SuppressFinalize(this);
    }
}