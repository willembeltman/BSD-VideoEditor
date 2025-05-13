using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D;

public class Default60FpsDrawerThread : IDrawerThread
{
    private readonly IApplicationForm ApplicationForm;
    private readonly IApplicationContext Application;
    private readonly Thread Thread;

    public virtual double FrameTime => 1d / 60d;

    public Default60FpsDrawerThread(IApplicationForm applicationForm, IApplicationContext application)
    {
        ApplicationForm = applicationForm;
        Application = application;
        Thread = new Thread(new ThreadStart(Kernel));
        Thread.Name = "DrawThread Kernel";
    }

    public void StartThread()
    {
        Thread.Start();
    }

    private void Kernel()
    {
        while (!Application.KillSwitch)
        {
            ApplicationForm.Timers.FpsTimer.SleepTillNextFrame(FrameTime);
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