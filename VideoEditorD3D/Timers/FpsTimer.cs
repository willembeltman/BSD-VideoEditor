using VideoEditorD3D.Interfaces;

namespace VideoEditorD3D.Timers;

public class FpsTimer
{
    public FpsTimer(IApplication application)
    {
        Application = application;
        FpsQueue = new Queue<double>();
    }

    private IApplication Application { get; }
    private Queue<double> FpsQueue { get; }

    public int Counter { get; private set; }

    //public void SleepTillNextFrame()
    //{
    //    var interval = 1d / Application.Config.RequestedFps;
    //    var currentTime = Application.Stopwatch.Elapsed.TotalSeconds;
    //    var waitInSeconds = interval - currentTime % interval;
    //    var wait = Convert.ToInt32(waitInSeconds * 1000);
    //    if (wait > 0)
    //        Thread.Sleep(wait);
    //}
    public void CountFps()
    {
        var currentTime = Application.Stopwatch.Elapsed.TotalSeconds;

        FpsQueue.Enqueue(currentTime);
        while (FpsQueue.Count(a => a < currentTime - 1) > 0)
            FpsQueue.Dequeue();

        Counter = FpsQueue.Count();
    }
}