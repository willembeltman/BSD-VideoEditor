using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class Label : Control
{
    public Label(IApplicationForm application, Form? parentForm, Control? parentControl) : base(application, parentForm, parentControl)
    {
        Background = CreateCanvasLayer();
        Foreground = CreateCanvasLayer();
        Border = CreateCanvasLayer();
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

    private RawColor4 _ForegroundColor = new RawColor4(1, 1, 1, 1);
    public RawColor4 ForegroundColor
    {
        get => _ForegroundColor;
        set
        {
            if (_ForegroundColor.Equals(value)) return;
            _ForegroundColor = value;
            Invalidate();
        }
    }

    private RawColor4 _BackgroundColor = new RawColor4(0, 0, 0, 0);
    public RawColor4 BackgroundColor
    {
        get => _BackgroundColor;
        set
        {
            if (_BackgroundColor.Equals(value)) return;
            _BackgroundColor = value;
            Invalidate();
        }
    }

    private RawColor4 _BorderColor = new RawColor4(1, 1, 1, 1);
    public RawColor4 BorderColor
    {
        get => _BorderColor;
        set
        {
            if (_BorderColor.Equals(value)) return;
            _BorderColor = value;
            Invalidate();
        }
    }

    private string _Font = "Arial";
    public string Font
    {
        get => _Font;
        set
        {
            if (_Font == value) return;
            _Font = value;
            Invalidate();
        }
    }

    private float _FontSize = 10;
    public float FontSize
    {
        get => _FontSize;
        set
        {
            if (_FontSize == value) return;
            _FontSize = value;
            Invalidate();
        }
    }

    private int _FontLetterSpacing = -2;
    public int FontLetterSpacing
    {
        get => _FontLetterSpacing;
        set
        {
            if (_FontLetterSpacing == value) return;
            _FontLetterSpacing = value;
            Invalidate();
        }
    }

    private FontStyle _FontStyle = FontStyle.Regular;
    public FontStyle FontStyle
    {
        get => _FontStyle;
        set
        {
            if (_FontStyle == value) return;
            _FontStyle = value;
            Invalidate();
        }
    }

    private int _BorderSize = 0;
    public int BorderSize
    {
        get => _BorderSize;
        set
        {
            if (_BorderSize == value) return;
            _BorderSize = value;
            Invalidate();
        }
    }

    public override void OnDraw()
    {
        Background.StartDrawing();
        Background.FillRectangle(Left, Top, Width, Height, BackgroundColor);
        Background.EndDrawing();

        Foreground.StartDrawing();
        Foreground.DrawText(Text, Left, Top, Width, Height, Font, FontSize, FontStyle, FontLetterSpacing, ForegroundColor);
        Foreground.EndDrawing();

        Border.StartDrawing();
        Border.DrawRectangle(Left, Top, Width, Height, BorderColor, BorderSize);
        Border.EndDrawing();
    }
}
