using System.Diagnostics;

namespace VideoEditorD3D.Timers;

public class CpuTimer
{
    private Queue<double> ElapsedQueue;
    private int MaxCount;
    private double StartTime;

    public CpuTimer(Stopwatch stopwatch, int count = 32)
    {
        Stopwatch = stopwatch;
        ElapsedQueue = new Queue<double>();
        MaxCount = count;
    }

    public Stopwatch Stopwatch { get; }
    public double Time { get; private set; }

    public void Start()
    {
        StartTime = Stopwatch.Elapsed.TotalSeconds;
    }
    public void Stop()
    {
        var currentTime = Stopwatch.Elapsed.TotalSeconds;
        var loadDataTime = currentTime - StartTime;

        ElapsedQueue.Enqueue(loadDataTime);
        while (ElapsedQueue.Count > MaxCount)
            ElapsedQueue.Dequeue();

        Time = ElapsedQueue.Average();
    }
}