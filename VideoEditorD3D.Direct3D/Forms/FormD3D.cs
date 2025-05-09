using SharpDX.Direct3D11;

namespace VideoEditorD3D.Direct3D.Forms;

public abstract class FormD3D(IApplication application) : ControlD3D(null)
{
    public override FormD3D ParentForm => this;
    public Device Device => application.Device;
    public Characters Characters => application.Characters;

    public Canvas GetCanvas()
    {
        return new Canvas(GetLayers(), BackgroundColor);
    }
}
