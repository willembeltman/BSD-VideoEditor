using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Controls.Templates;

namespace VideoEditorD3D.Direct3D.Controls;

public class Label : ForeBorderBackControl
{
    public Label(IApplicationForm application) : base(application)
    {
        Foreground = GraphicsLayers.CreateNewLayer();
    }

    protected readonly GraphicsLayer Foreground;

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
        Foreground.StartDrawing();
        Foreground.DrawText(Text, TextPaddingLeft, TextPaddingTop, TextPaddingLeft + Width + TextPaddingRight, TextPaddingTop + Height + TextPaddingBottom, Font, FontSize, FontStyle, FontLetterSpacing, ForeColor);
        Foreground.EndDrawing();

        base.OnDraw();
    }
}
