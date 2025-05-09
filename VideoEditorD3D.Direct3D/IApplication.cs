using SharpDX.Direct3D11;
using System.Diagnostics;
using VideoEditorD3D.Configs;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Entities;
using VideoEditorD3D.Loggers;
using VideoEditorD3D.Timers;

namespace VideoEditorD3D.Direct3D
{
    public interface IApplication
    {
        Characters Characters { get; }
        ApplicationConfig Config { get; }
        FormD3D CurrentForm { get; set; }
        ApplicationDbContext Db { get; }
        Device Device { get; }
        ConsoleLogger Logger { get; }
        int PhysicalHeight { get; }
        int PhysicalWidth { get; }
        Stopwatch Stopwatch { get; }
        AllTimers Timers { get; }

        void Draw();
    }
}