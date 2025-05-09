using VideoEditorD3D.Direct3D;

namespace VideoEditorD3D;

public class Router : IDisposable
{
    public Router(IApplication application)
    {
        Application = application;
        DrawThread = new Thread(new ThreadStart(DrawKernel));
    }

    private readonly IApplication Application;
    private readonly Thread DrawThread;
    private bool KillSwitch;

    public void StartThread()
    {
        DrawThread.Start();
    }

    private void DrawKernel()
    {
        while (!KillSwitch)
        {
            Application.Draw();
            //Thread.Sleep(16);
        }
    }

    public void Dispose()
    {
        KillSwitch = true;
        if (DrawThread != null && DrawThread != Thread.CurrentThread && DrawThread.ThreadState == ThreadState.Running)
        {
            DrawThread.Join();
        }
        GC.SuppressFinalize(this);
    }
}