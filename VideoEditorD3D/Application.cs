namespace VideoEditorD3D;

public class Application : IDisposable
{
    public Application(ApplicationContext context)
    {
        Context = context;
        Thread = new Thread(new ThreadStart(Kernel));
    }

    private readonly ApplicationContext Context;
    private readonly Thread Thread;
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
        Thread.Start();
    }

    private void Kernel()
    {
        while (!KillSwitch)
        {
            Context.Draw();
            Thread.Sleep(16);
        }
    }

    public void Dispose()
    {
        KillSwitch = true;
        if (Thread != null && Thread != Thread.CurrentThread && Thread.ThreadState == ThreadState.Running)
        {
            Thread.Join();
        }
        GC.SuppressFinalize(this);
    }
}