namespace VideoEditorD3D;

public class Router : IDisposable
{
    public Router(Application context)
    {
        Application = context;
        DrawThread = new Thread(new ThreadStart(DrawKernel));
    }

    private readonly Application Application;
    private readonly Thread DrawThread;
    private bool KillSwitch;

    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
    }

    public void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
    }

    public void OnKeyUp(object? sender, KeyEventArgs e)
    {
    }

    public void OnMouseWheel(object? sender, MouseEventArgs e)
    {
    }

    public void OnMouseMove(object? sender, MouseEventArgs e)
    {
    }

    public void MouseDown(object? sender, MouseEventArgs e)
    {
    }

    public void MouseUp(object? sender, MouseEventArgs e)
    {
    }

    public void MouseClick(object? sender, MouseEventArgs e)
    {
    }

    public void StartThread()
    {
        DrawThread.Start();
    }

    private void DrawKernel()
    {
        while (!KillSwitch)
        {
            Application.Draw();
            Thread.Sleep(16);
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