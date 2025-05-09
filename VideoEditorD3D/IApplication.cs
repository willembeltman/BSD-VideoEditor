using System.Diagnostics;
using VideoEditorD3D.Configs;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Entities;
using VideoEditorD3D.Loggers;
using VideoEditorD3D.Timers;

namespace VideoEditorD3D;

public interface IApplication : IApplicationD3D
{
    ApplicationConfig Config { get; }
    FormD3D CurrentForm { get; set; }
    ApplicationDbContext Db { get; }
    ConsoleLogger Logger { get; }
    Stopwatch Stopwatch { get; }
    AllTimers Timers { get; }
    bool KillSwitch { get; set; }

    void Draw();
}