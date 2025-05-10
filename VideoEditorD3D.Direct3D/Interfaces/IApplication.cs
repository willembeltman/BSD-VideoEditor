using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Loggers;

namespace VideoEditorD3D.Direct3D.Interfaces;

public interface IApplication : IDisposable
{
    ILogger Logger { get; }
    bool KillSwitch { get; set; }

    IDrawerThread? OnCreateDrawerThread(IApplicationForm applicationForm);
    Forms.Form OnCreateStartForm(IApplicationForm applicationForm);
}