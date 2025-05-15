using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Controls.Templates;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class Form : BackControl
{
    public Form(IApplicationForm applicationForm) : base(applicationForm)
    {
        Controls = new ControlCollection(this);
    }
}
