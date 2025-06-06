using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Events;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls;

public class Button : Label
{
    private RawColor4? OriginalBackColor;
    private RawColor4? OriginalForeColor;

    private RawColor4 _MouseDownBackColor = new(1, 1, 1, 1);
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

    private RawColor4 _MouseDownForeColor = new(0, 0, 0, 1);
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

    public override RawColor4 BackColor
    {
        get => base.BackColor;
        set
        {
            base.BackColor = value;
            if (!MouseIsDown)
                OriginalBackColor = value;
        }
    }
    public override RawColor4 ForeColor
    {
        get => base.ForeColor;
        set
        {
            base.ForeColor = value;
            if (!MouseIsDown)
                OriginalForeColor = value;
        }
    }

    public Button()
    {
        BorderSize = 2;
        MouseDown += Button_MouseDown;
        MouseUp += Button_MouseUp;
        MouseLeave += Button_MouseLeave;
        BackColor = new RawColor4(0, 0, 0, 0.5f);
        ForeColor = new RawColor4(1, 1, 1, 1);
    }

    private void Button_MouseDown(object? sender, MouseEvent e)
    {
        if (OriginalBackColor != null && OriginalForeColor != null)
        {
            BackColor = MouseDownBackColor;
            ForeColor = MouseDownForeColor;
        }
    }
    private void Button_MouseUp(object? sender, MouseEvent e)
    {
        Button_MouseLeave(sender, e);
    }
    private void Button_MouseLeave(object? sender, EventArgs e)
    {
        if (OriginalBackColor != null && OriginalForeColor != null)
        {
            BackColor = OriginalBackColor.Value;
            ForeColor = OriginalForeColor.Value;
        }
    }
}
