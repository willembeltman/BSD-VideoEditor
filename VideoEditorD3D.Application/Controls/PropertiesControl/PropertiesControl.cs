using VideoEditorD3D.Direct3D.Drawing;

namespace VideoEditorD3D.Application.Controls.PropertiesControl;

public class PropertiesControl : BaseControl
{
    private readonly GraphicsLayer Foreground;

    public PropertiesControl()
    {
        Foreground = GraphicsLayers.CreateNewLayer();
        Draw += PropertiesControl_Draw;
    }

    private void PropertiesControl_Draw(object? sender, EventArgs e)
    {
        Foreground.StartDrawing();
        Foreground.FillRectangle(0, 0, Width, Height, BackColor);
        Foreground.EndDrawing();
    }
}
