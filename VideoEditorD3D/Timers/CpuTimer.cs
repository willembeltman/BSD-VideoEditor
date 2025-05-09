using VideoEditorD3D.Interfaces;

namespace VideoEditorD3D.Timers;

public class CpuTimer
{
    private Queue<double> ElapsedQueue;
    private int MaxCount;
    private double StartTime;

    public CpuTimer(IApplication application, int count = 32)
    {
        Application = application;
        ElapsedQueue = new Queue<double>();
        MaxCount = count;
    }

    public IApplication Application { get; }
    public double Time { get; private set; }

    public void Start()
    {
        StartTime = Application.Stopwatch.Elapsed.TotalSeconds;
    }
    public void Stop()
    {
        var currentTime = Application.Stopwatch.Elapsed.TotalSeconds;
        var loadDataTime = currentTime - StartTime;

        ElapsedQueue.Enqueue(loadDataTime);
        while (ElapsedQueue.Count > MaxCount)
            ElapsedQueue.Dequeue();

        Time = ElapsedQueue.Average();
    }
}