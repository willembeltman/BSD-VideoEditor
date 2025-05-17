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

            var size = Foreground.MeasureText(Text, -1, -1, Font, FontSize, FontStyle, FontLetterSpacing, ForeColor);
            Width = TextPaddingLeft + TextPaddingRight + size.Width;
            Height = TextPaddingTop + TextPaddingBottom + size.Height;

            Invalidate();
            //ParentControl.Invalidate();
        }
    }

    public override void OnDraw()
    {
        Foreground.StartDrawing();
        var size = Foreground.DrawText(Text, TextPaddingLeft, TextPaddingTop, -1, -1, Font, FontSize, FontStyle, FontLetterSpacing, ForeColor);
        Width = TextPaddingLeft + TextPaddingRight + size.Width;
        Height = TextPaddingTop + TextPaddingBottom + size.Height;
        Foreground.EndDrawing();

        base.OnDraw();
    }
}
