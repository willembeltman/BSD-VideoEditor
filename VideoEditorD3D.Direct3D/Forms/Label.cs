using VideoEditorD3D.Direct3D.Forms.Generic;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Drawing;

namespace VideoEditorD3D.Direct3D.Forms;

public class Label : ForeBorderBackControl
{
    public Label(IApplicationForm application) : base(application)
    {
        Background = CanvasLayers.CreateNewLayer();
        Foreground = CanvasLayers.CreateNewLayer();
        Border = CanvasLayers.CreateNewLayer();
    }

    private readonly GraphicsLayer Background;
    private readonly GraphicsLayer Foreground;
    private readonly GraphicsLayer Border;

    private string _Text = string.Empty;
    public string Text
    {
        get => _Text;
        set
        {
            if (_Text == value) return;
            _Text = value;
            Invalidate();
        }
    }

    public override void OnDraw()
    {
        Background.StartDrawing();
        Background.FillRectangle(0, 0, Width, Height, BackColor);
        Background.EndDrawing();

        Foreground.StartDrawing();
        Foreground.DrawText(Text, 0, 0, Width, Height, Font, FontSize, FontStyle, FontLetterSpacing, ForeColor);
        Foreground.EndDrawing();

        Border.StartDrawing();
        Border.DrawRectangle(0, 0, Width, Height, BorderColor, BorderSize);
        Border.EndDrawing();

        base.OnDraw();
    }
}
