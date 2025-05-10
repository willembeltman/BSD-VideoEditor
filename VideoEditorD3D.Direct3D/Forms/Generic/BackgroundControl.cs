using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms.Generic;

public class BackgroundControl : Control
{
    public BackgroundControl(IApplicationForm application, Form? parentForm, Control? parentControl) : base(application, parentForm, parentControl)
    {
    }

    private RawColor4 _BackgroundColor = new RawColor4(0, 0, 0, 0);
    public RawColor4 BackgroundColor
    {
        get => _BackgroundColor;
        set
        {
            if (_BackgroundColor.Equals(value)) return;
            _BackgroundColor = value;
            Invalidate();
        }
    }  
}
