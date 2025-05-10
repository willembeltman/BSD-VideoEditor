using System.Diagnostics;
using SharpDX.Direct3D11;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Timers;

namespace VideoEditorD3D.Direct3D.Interfaces;

public interface IApplicationForm
{
    Device Device { get; }
    int Width { get; }
    int Height { get; }

    CharacterCollection Characters { get; }
    FormD3D CurrentForm { get; set; }
    Stopwatch Stopwatch { get; }
    AllTimers Timers { get; }

    void Draw();
}