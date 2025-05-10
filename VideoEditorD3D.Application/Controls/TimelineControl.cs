using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Application.Controls;

public class TimelineControl : Control
{
    public TimelineControl(ApplicationContext application, IApplicationForm applicationForm, Form? parentForm, Control? parentControl) : base(applicationForm, parentForm, parentControl)
    {
        Background = CreateCanvasLayer();
        Foreground = CreateCanvasLayer();
    }

    private readonly GraphicsLayer Background;
    private readonly GraphicsLayer Foreground;


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

    public override void OnDraw()
    {
        Background.StartDrawing();
        Background.FillRectangle(Left, Top, Width, Height, BackgroundColor);
        Background.EndDrawing();
    }
}
