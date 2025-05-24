using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Timers;

public class Default60FpsDrawerThread : IDrawerThread
{
    private readonly IApplicationForm ApplicationForm;
    private readonly Thread Thread;

    public virtual double FrameTime => 1d / 60d;

    public bool KillSwitch { get; private set; }

    public Default60FpsDrawerThread(IApplicationForm applicationForm)
    {
        ApplicationForm = applicationForm;
        Thread = new Thread(new ThreadStart(Kernel));
        Thread.Name = "DrawThread Kernel";
    }

    public void StartThread()
    {
        Thread.Start();
    }

    private void Kernel()
    {
        while (!ApplicationForm.KillSwitch && !KillSwitch)
        {
            ApplicationForm.Timers.FpsTimer.SleepTillNextFrame(FrameTime);
            ApplicationForm.TryDraw();
        }
    }

    public void Dispose()
    {
        Dispose(true);

        KillSwitch = true;
        if (Thread != null && Thread != Thread.CurrentThread && Thread.ThreadState == ThreadState.Running)
        {
            Thread.Join();
        }
        GC.SuppressFinalize(this);
    }
    public virtual void Dispose(bool disposing)
    {

    }
}