using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Forms.Generic;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Application.Controls;

public class TimelineControl : BackgroundControl
{
    private readonly GraphicsLayer Background;
    private readonly GraphicsLayer Foreground;
    private readonly HScrollBar HScrollBar;

    public TimelineControl(ApplicationContext application, IApplicationForm applicationForm, Form? parentForm, Control? parentControl) : base(applicationForm, parentForm, parentControl)
    {
        Background = CreateCanvasLayer();
        Foreground = CreateCanvasLayer();

        HScrollBar = new HScrollBar(applicationForm, parentForm, this);
        AddControl(HScrollBar);
    }

    public override void OnResize()
    {
        var marge = 0;
        HScrollBar.Height = 50;
        HScrollBar.Left = marge;
        HScrollBar.Width = Width - marge * 2;
        HScrollBar.Top = Height - HScrollBar.Height;
        base.OnResize();
    }

    public override void OnDraw()
    {
        Background.StartDrawing();
        Background.FillRectangle(0, 0, Width, Height, BackgroundColor);
        Background.EndDrawing();
    }
}
