using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Drawing;

namespace VideoEditorD3D.Direct3D.Controls.Templates;

public class BackControl : Control
{
    private readonly GraphicsLayer BackgroundLayer;

    public BackControl()
    {
        BackgroundLayer = GraphicsLayers.CreateNewLayer();
        Draw += BackControl_Draw;
        Resize += BackControl_Resize;
    }

    private void BackControl_Resize(object? sender, EventArgs e)
    {
        Invalidate();
    }

    private RawColor4 _BackColor = new(0, 0, 0, 0);
    public RawColor4 BackColor
    {
        get => _BackColor;
        set
        {
            if (_BackColor.Equals(value)) return;
            _BackColor = value;
            Invalidate();
        }
    }

    private void BackControl_Draw(object? sender, EventArgs e)
    {
        BackgroundLayer.StartDrawing();
        if (BackColor.A > 0)
        {
            BackgroundLayer.FillRectangle(0, 0, Width, Height, BackColor);
        }
        BackgroundLayer.EndDrawing();
    }
}
