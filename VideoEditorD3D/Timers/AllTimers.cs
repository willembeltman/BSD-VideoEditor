using VideoEditorD3D.Interfaces;

namespace VideoEditorD3D.Timers
{
    public class AllTimers
    {
        public AllTimers(IApplication application)
        {
            FpsTimer = new FpsTimer(application);
            PpsTimer = new FpsTimer(application);
            LongBotTimer = new CpuTimer(application);
            ShortBotTimer = new CpuTimer(application);
            UpdateFormTimer = new CpuTimer(application);
            GraphLoadModelTimer = new CpuTimer(application);
            GraphDrawCanvasTimer = new CpuTimer(application);
            GraphCompileCanvasTimer = new CpuTimer(application);
            GraphDrawToGpuTimer = new CpuTimer(application);
        }
        public FpsTimer FpsTimer { get; }
        public FpsTimer PpsTimer { get; }
        public CpuTimer LongBotTimer { get; }
        public CpuTimer ShortBotTimer { get; }
        public CpuTimer UpdateFormTimer { get; }
        public CpuTimer GraphLoadModelTimer { get; }
        public CpuTimer GraphDrawCanvasTimer { get; }
        public CpuTimer GraphCompileCanvasTimer { get; }
        public CpuTimer GraphDrawToGpuTimer { get; }
    }
}