using System.Diagnostics;
using VideoEditorD3D.Configs;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Entities;
using VideoEditorD3D.Loggers;
using VideoEditorD3D.Timers;

namespace VideoEditorD3D;

public interface IApplication : Direct3D.Interfaces.IApplication
{
    ConsoleLogger Logger { get; }
    ApplicationConfig Config { get; }
    ApplicationDbContext Db { get; }
    FormD3D CurrentForm { get; set; }
    Stopwatch Stopwatch { get; }
    AllTimers Timers { get; }
    bool KillSwitch { get; set; }

    void Draw();
}