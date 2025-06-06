using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Controls.Templates;

namespace VideoEditorD3D.Direct3D.Controls;

public class Form : BackControl
{
    public Form()
    {
        Controls = new ControlCollection(this);
    }
}
