using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls.Templates;

public class BorderBackControl : BackControl
{
    private RawColor4 _BorderColor = new RawColor4(1, 1, 1, 1);
    private int _BorderSize = 1;
    private readonly GraphicsLayer Border;

    public BorderBackControl(IApplicationForm application) : base(application)
    {
        Border = GraphicsLayers.CreateNewLayer();
    }

    public event EventHandler<RawColor4>? BorderColorChanged;
    public RawColor4 BorderColor
    {
        get => _BorderColor;
        set
        {
            if (_BorderColor.Equals(value)) return;
            _BorderColor = value;
            BorderColorChanged?.Invoke(this, BorderColor);
            Invalidate();
        }
    }
    public event EventHandler<int>? BorderSizeChanged;
    public int BorderSize
    {
        get => _BorderSize;
        set
        {
            if (_BorderSize == value) return;
            _BorderSize = value;
            BorderSizeChanged?.Invoke(this, BorderSize);
            Invalidate();
        }
    }
    public override void OnDraw()
    {
        if (BorderColor.A > 0)
        {
            Border.StartDrawing();
            Border.DrawRectangle(0, 0, Width, Height, BorderColor, BorderSize);
            Border.EndDrawing();
        }

        base.OnDraw();
    }
}
