using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms.Generic;

namespace VideoEditorD3D.Application.Controls;

public class PropertiesControl : BackControl
{
    public PropertiesControl(ApplicationContext applicationContext, IApplicationForm applicationForm, Form? parentForm, Control? parentControl) : base(applicationForm, parentForm, parentControl)
    {
        Background = CanvasLayers.Create();
        Foreground = CanvasLayers.Create();
    }

    private readonly GraphicsLayer Background;
    private readonly GraphicsLayer Foreground;

    public override void OnDraw()
    {
        Background.StartDrawing();
        Background.FillRectangle(0, 0, Width, Height, BackColor);
        Background.EndDrawing();
        base.OnDraw();
    }
}
