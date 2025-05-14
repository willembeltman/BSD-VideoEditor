using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls.Generic;

public class BorderBackControl(IApplicationForm application) : BackControl(application)
{
    private RawColor4 _BorderColor = new RawColor4(1, 1, 1, 1);
    private int _BorderSize = 0;
    public RawColor4 BorderColor
    {
        get => _BorderColor;
        set
        {
            if (_BorderColor.Equals(value)) return;
            _BorderColor = value;
            Invalidate();
        }
    }
    public int BorderSize
    {
        get => _BorderSize;
        set
        {
            if (_BorderSize == value) return;
            _BorderSize = value;
            Invalidate();
        }
    }
}
