using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Controls.Templates;

namespace VideoEditorD3D.Application.Controls;

public class PropertiesControl : BackControl
{
    public PropertiesControl(ApplicationContext applicationContext, IApplicationForm applicationForm) : base(applicationForm)
    {
        Foreground = GraphicsLayers.CreateNewLayer();
        Foreground = GraphicsLayers.CreateNewLayer();
    }

    private readonly GraphicsLayer Foreground;

    public override void OnDraw()
    {
        Foreground.StartDrawing();
        Foreground.FillRectangle(0, 0, Width, Height, BackColor);
        Foreground.EndDrawing();
        base.OnDraw();
    }
}
