using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms.Generic;

namespace VideoEditorD3D.Application.Controls;

public class PropertiesControl : BackgroundControl
{
    public PropertiesControl(ApplicationContext application, IApplicationForm applicationForm, Form? parentForm, Control? parentControl) : base(applicationForm, parentForm, parentControl)
    {
        Background = CreateCanvasLayer();
        Foreground = CreateCanvasLayer();
    }

    private readonly GraphicsLayer Background;
    private readonly GraphicsLayer Foreground;

    public override void OnDraw()
    {
        Background.StartDrawing();
        Background.FillRectangle(0, 0, Width, Height, BackgroundColor);
        Background.EndDrawing();
    }
}
