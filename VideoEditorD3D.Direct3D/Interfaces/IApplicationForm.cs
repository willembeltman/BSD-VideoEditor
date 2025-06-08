using SharpDX.Direct3D11;
using System.Diagnostics;
using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Timers;
using FormCollection = VideoEditorD3D.Direct3D.Collections.FormCollection;

namespace VideoEditorD3D.Direct3D.Interfaces;

public interface IApplicationForm
{
    IApplicationState ApplicationContext { get; }
    Device Device { get; }
    bool KillSwitch { get; set; }
    int Width { get; }
    int Height { get; }

    CharacterCollection Characters { get; }
    FormCollection Forms { get; }
    Stopwatch Stopwatch { get; }
    AllTimers Timers { get; }
    Cursor Cursor { get; set; }

    void TryDraw();
    void CloseForm();
}