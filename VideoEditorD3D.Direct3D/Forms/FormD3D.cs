
using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class FormD3D(IApplication application) : ControlD3D(application, null, null), ICanvas
{
}
