using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls;

public class Button : Label
{
    public Button(IApplicationForm application) : base(application)
    {
        BorderSize = 2;
    }

    private RawColor4 _MouseDownBackColor = new RawColor4(1, 1, 1, 1);
    public RawColor4 MouseDownBackColor
    {
        get => _MouseDownBackColor;
        set
        {
            if (_MouseDownBackColor.Equals(value)) return;
            _MouseDownBackColor = value;
            Invalidate();
        }
    }
    private RawColor4 _MouseDownForeColor = new RawColor4(0, 0, 0, 1);
    public RawColor4 MouseDownForeColor
    {
        get => _MouseDownForeColor;
        set
        {
            if (_MouseDownForeColor.Equals(value)) return;
            _MouseDownForeColor = value;
            Invalidate();
        }
    }

    public RawColor4? OriginalBackColor { get; private set; }
    public RawColor4? OriginalForeColor { get; private set; }

    public override void OnMouseDown(MouseEventArgs e)
    {
        if (OriginalBackColor == null && OriginalForeColor == null)
        {
            OriginalBackColor = BackColor;
            OriginalForeColor = ForeColor;
            BackColor = MouseDownBackColor;
            ForeColor = MouseDownForeColor;
            Invalidate();
        }
        base.OnMouseClick(e);
    }
    public override void OnMouseUp(MouseEventArgs e)
    {
        OnMouseLeave(e);
        base.OnMouseClick(e);
    }
    public override void OnMouseLeave(EventArgs e)
    {
        if (OriginalBackColor != null && OriginalForeColor != null)
        {
            BackColor = OriginalBackColor.Value;
            ForeColor = OriginalForeColor.Value;
            OriginalBackColor = null;
            OriginalForeColor = null;
            Invalidate();
        }
        base.OnMouseLeave(e);
    }
}
