using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class Form(IApplicationForm applicationForm) : Control(applicationForm, null, null)
{
    private RawColor4 _BackgroundColor = new RawColor4(0, 0, 0, 1);
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
