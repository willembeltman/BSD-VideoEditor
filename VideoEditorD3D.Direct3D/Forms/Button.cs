using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class Button : Label
{
    public Button(IApplicationForm application, Form? parentForm, Control? parentControl) : base(application, parentForm, parentControl)
    {
        BorderSize = 1;
    }
}
