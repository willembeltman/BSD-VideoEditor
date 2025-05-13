using System.Diagnostics;

namespace VideoEditorD3D.Direct3D.Timers;

public class CpuTimer
{
    private readonly Stopwatch Stopwatch;
    private readonly Queue<double> ElapsedQueue;
    private readonly int MaxCount;
    private double StartTime;

    public CpuTimer(Stopwatch stopwatch, int count = 32)
    {
        Stopwatch = stopwatch;
        ElapsedQueue = new Queue<double>();
        MaxCount = count;
    }

    public double Time { get; private set; }

    public void Start()
    {
        StartTime = Stopwatch.Elapsed.TotalSeconds;
    }
    public void Stop()
    {
        var currentTime = Stopwatch.Elapsed.TotalSeconds;
        var timeSpend = currentTime - StartTime;
        AddNewTime(timeSpend);
    }

    private void AddNewTime(double timeSpend)
    {
        ElapsedQueue.Enqueue(timeSpend);
        while (ElapsedQueue.Count > MaxCount)
            ElapsedQueue.Dequeue();

        Time = ElapsedQueue.Average();
    }

    public CpuTimerDisposableObject DisposableObject
    {
        get
        {
            var cpuTimerObject = new CpuTimerDisposableObject(this);
            return cpuTimerObject;
        }
    }
    public class CpuTimerDisposableObject : IDisposable
    {
        public CpuTimerDisposableObject(CpuTimer cpuTimer)
        {
            CpuTimer = cpuTimer;
            StartTime = CpuTimer.Stopwatch.Elapsed.TotalSeconds;
        }

        public CpuTimer CpuTimer { get; }
        public double StartTime { get; }

        public void Dispose()
        {
            var currentTime = CpuTimer.Stopwatch.Elapsed.TotalSeconds;
            var timeSpend = currentTime - StartTime;
            CpuTimer.AddNewTime(timeSpend);
        }
    }
}
