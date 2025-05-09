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
        ApplicationConfig Config { get; }
        FormD3D CurrentForm { get; set; }
        ApplicationDbContext Db { get; }
        ConsoleLogger Logger { get; }
        Stopwatch Stopwatch { get; }
        AllTimers Timers { get; }
        Device Device { get; }
        Characters Characters { get; }
        int Width { get; }
        int Height { get; }

        void Draw();
    }
}