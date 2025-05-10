using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms.Generic;

public class BackControl : Control
{
    public BackControl(IApplicationForm application, Form? parentForm, Control? parentControl) : base(application, parentForm, parentControl)
    {
    }

    private RawColor4 _BackColor = new RawColor4(0, 0, 0, 0);
    public RawColor4 BackColor
    {
        get => _BackColor;
        set
        {
            if (_BackColor.Equals(value)) return;
            _BackColor = value;
            Invalidate();
        }
    }  
}
