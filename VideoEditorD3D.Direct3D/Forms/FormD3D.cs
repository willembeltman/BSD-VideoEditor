using SharpDX.Direct3D11;

namespace VideoEditorD3D.Direct3D.Forms;

public abstract class FormD3D(IApplication application) : ControlD3D(null), ICanvas
{
    public IApplication Application { get; } = application;
    public override FormD3D ParentForm => this;
}
