using SharpDX.Direct3D11;
using System.Diagnostics;
using VideoEditorD3D.Configs;
using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Engine;
using VideoEditorD3D.Loggers;
using VideoEditorD3D.Timers;

namespace VideoEditorD3D.Interfaces
{
    public interface IApplication
    {
        ApplicationConfig Config { get; }
        ProjectDbContext Database { get; }
        Device Device { get; }
        ConsoleLogger Logger { get; }
        Stopwatch Stopwatch { get; }
        CharacterCollection Characters { get; }
        Drawer Drawer { get; }
        Timeline Timeline { get; }
        AllTimers Timers { get; }

        int Height { get; }
        int Width { get; }

        void Draw();
    }
}