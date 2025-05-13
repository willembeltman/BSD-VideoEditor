using System.Diagnostics;

namespace VideoEditorD3D.Direct3D.Timers;

public class AllTimers(Stopwatch stopwatch)
{
    public FpsTimer FpsTimer { get; } = new FpsTimer(stopwatch);
    public CpuTimer OnUpdateTimer { get; } = new CpuTimer(stopwatch);
    public CpuTimer RenderToGpuTimer { get; } = new CpuTimer(stopwatch);
}