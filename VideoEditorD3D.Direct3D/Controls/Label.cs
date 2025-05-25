using VideoEditorD3D.Direct3D.Controls.Templates;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls;

public class Label : ForeBorderBackControl
{
    protected readonly GraphicsLayer Foreground;

    public Label()
    {
        Foreground = GraphicsLayers.CreateNewLayer();

        Draw += Label_Draw;
    }

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

    private void Label_Draw(object? sender, EventArgs e)
    {
        Foreground.StartDrawing();
        //var size = 
        Foreground.DrawText(Text, TextPaddingLeft, TextPaddingTop, -1, -1, Font, FontSize, FontStyle, FontLetterSpacing, ForeColor);
        //Width = TextPaddingLeft + TextPaddingRight + size.Width;
        //Height = TextPaddingTop + TextPaddingBottom + size.Height;
        Foreground.EndDrawing();
    }
}
