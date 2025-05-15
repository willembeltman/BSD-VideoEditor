using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls.Templates;

public class BackControl : Control
{
    private readonly GraphicsLayer BackgroundLayer;

    public BackControl(IApplicationForm application) : base(application)
    {
        BackgroundLayer = GraphicsLayers.CreateNewLayer();
    }

    private RawColor4 _BackColor = new(0, 0, 0, 0);

    public RawColor4 OldBackColor { get; private set; }

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

    public override void OnDraw()
    {
        if (BackColor.A > 0)
        {
            BackgroundLayer.StartDrawing();
            BackgroundLayer.FillRectangle(0, 0, Width, Height, BackColor);
            BackgroundLayer.EndDrawing();
        }

        base.OnDraw();
    }
}
