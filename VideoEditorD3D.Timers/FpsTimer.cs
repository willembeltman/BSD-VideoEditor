using System.Diagnostics;
using VideoEditorD3D.Types;

namespace VideoEditorD3D.Timers;

public class FpsTimer
{
    private readonly Stopwatch Stopwatch;
    private readonly Queue<double> FpsQueue;
    public int Fps { get; private set; }

    public FpsTimer(Stopwatch stopwatch)
    {
        Stopwatch = stopwatch;
        FpsQueue = new Queue<double>();
    }

    public void SleepTillNextFrame(Fps fps)
    {
        var interval = 1d * fps.Base / fps.Divider;
        var currentTime = Stopwatch.Elapsed.TotalSeconds;
        var waitInSeconds = interval - currentTime % interval;
        var wait = Convert.ToInt32(waitInSeconds * 1000);
        if (wait > 0)
            Thread.Sleep(wait);
    }
    public void CountFps()
    {
        var currentTime = Stopwatch.Elapsed.TotalSeconds;

        FpsQueue.Enqueue(currentTime);
        while (FpsQueue.Count(a => a < currentTime - 1) > 0)
            FpsQueue.Dequeue();

        Fps = FpsQueue.Count();
    }
}