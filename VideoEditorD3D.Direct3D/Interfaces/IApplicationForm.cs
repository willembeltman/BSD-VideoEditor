using System.Diagnostics;
using SharpDX.Direct3D11;
using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Timers;

namespace VideoEditorD3D.Direct3D.Interfaces;

public interface IApplicationForm
{
    Device Device { get; }
    int Width { get; }
    int Height { get; }

    CharacterCollection Characters { get; }
    Forms.Form CurrentForm { get; set; }
    PopupCollection Popups { get; }
    Stopwatch Stopwatch { get; }
    AllTimers Timers { get; }
    Cursor Cursor { get; set; }

    void TryDraw();
    void CloseForm();
    Point PointToClient(Point formPoint);
    void EnableDragAndDrop();
}