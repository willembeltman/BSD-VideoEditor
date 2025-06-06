using VideoEditorD3D.Entities;

namespace VideoEditorD3D.Application.Buffers;

public class AudioBuffer : IDisposable
{
    private readonly Timeline Timeline;
    private readonly TimelineClipAudio AudioClip;
    private readonly Thread Thread;

    public bool KillSwitch { get; private set; }

    public AudioBuffer(Timeline timeline, TimelineClipAudio audioClip)
    {
        Timeline = timeline;
        Timeline.CurrentTimeUpdated += Timeline_CurrentTimeUpdated;
        AudioClip = audioClip;
        Thread = new Thread(new ThreadStart(Kernel));
    }

    public void StartThread()
    {
        Thread.Start();
    }

    private void Kernel()
    {
        //while (!KillSwitch)
        {
            //throw new NotImplementedException();
        }
    }

    private void Timeline_CurrentTimeUpdated(object? sender, double e)
    {
        // Don't know if needed because the thread should be running independently
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