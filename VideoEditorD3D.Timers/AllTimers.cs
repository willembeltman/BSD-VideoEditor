using System.Diagnostics;

namespace VideoEditorD3D.Timers;

public class AllTimers(Stopwatch stopwatch)
{
    public FpsTimer FpsTimer { get; } = new FpsTimer(stopwatch);
    public FpsTimer PpsTimer { get; } = new FpsTimer(stopwatch);
    public CpuTimer LongBotTimer { get; } = new CpuTimer(stopwatch);
    public CpuTimer ShortBotTimer { get; } = new CpuTimer(stopwatch);
    public CpuTimer UpdateFormTimer { get; } = new CpuTimer(stopwatch);
    public CpuTimer GraphLoadModelTimer { get; } = new CpuTimer(stopwatch);
    public CpuTimer GraphDrawCanvasTimer { get; } = new CpuTimer(stopwatch);
    public CpuTimer GraphCompileCanvasTimer { get; } = new CpuTimer(stopwatch);
    public CpuTimer GraphDrawToGpuTimer { get; } = new CpuTimer(stopwatch);
}