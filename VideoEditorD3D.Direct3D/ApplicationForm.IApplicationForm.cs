using SharpDX.Direct3D11;
using System.Diagnostics;
using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Timers;
using FormCollection = VideoEditorD3D.Direct3D.Collections.FormCollection;

namespace VideoEditorD3D.Direct3D
{
    public partial class ApplicationForm : IApplicationForm
    {
        bool IApplicationForm.KillSwitch { get => KillSwitch; set => KillSwitch = value; }
        IApplicationState IApplicationForm.ApplicationContext => ApplicationContext;
        Device IApplicationForm.Device => _Device!;
        CharacterCollection IApplicationForm.Characters => _Characters!;
        int IApplicationForm.Width => _PhysicalWidth!.Value;
        int IApplicationForm.Height => _PhysicalHeight!.Value;
        Stopwatch IApplicationForm.Stopwatch => Stopwatch;
        AllTimers IApplicationForm.Timers => Timers;
        FormCollection IApplicationForm.Forms => _Forms!;
    }
}
