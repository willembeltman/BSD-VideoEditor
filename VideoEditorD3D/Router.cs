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

    public void StartThread()
    {
        DrawThread.Start();
    }

    private void DrawKernel()
    {
        while (!Application.KillSwitch)
        {
            Application.Draw();
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