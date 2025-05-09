using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class FormD3D(IApplicationForm applicationForm) : ControlD3D(applicationForm, null, null)
{
    private RawColor4 _BackgroundColor;
    public RawColor4 BackgroundColor
    {
        get => _BackgroundColor;
        set
        {
            _BackgroundColor = value;
            Invalidate();
        }
    }

}
