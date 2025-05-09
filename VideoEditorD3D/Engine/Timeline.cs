using VideoEditorD3D.Interfaces;

namespace VideoEditorD3D.Engine;

public class Timeline : IDisposable
{
    private IApplication Application;

    public Timeline(IApplication application)
    {
        Application = application;
        Thread = new Thread(new ThreadStart(Kernel));
    }

    public Thread Thread { get; private set; }
    public bool KillSwitch { get; private set; }

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
        // Main player loop
        while (!KillSwitch)
        {
            Thread.Sleep(10);
        }
    }

    public void Dispose()
    {
        KillSwitch = true;
        if (Thread != null && Thread != Thread.CurrentThread && Thread.ThreadState == ThreadState.Running)
        {
            Thread.Join();
        }
    }

}