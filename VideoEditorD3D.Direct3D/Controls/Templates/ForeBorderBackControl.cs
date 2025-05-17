using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls.Templates;

public class ForeBorderBackControl(IApplicationForm application) : BorderBackControl(application)
{


    public event EventHandler<RawColor4>? FontColorChanged;
    private RawColor4 _ForeColor = new RawColor4(0, 0, 0, 0);
    public RawColor4 ForeColor
    {
        get => _ForeColor;
        set
        {
            if (_ForeColor.Equals(value)) return;
            _ForeColor = value;
            FontColorChanged?.Invoke(this, _ForeColor);
            Invalidate();
        }
    }

    public event EventHandler<string>? FontChanged;
    private string _Font = "Ebrima";
    public string Font
    {
        get => _Font;
        set
        {
            if (_Font == value) return;
            _Font = value;
            FontChanged?.Invoke(this, _Font);
            Invalidate();
        }
    }

    public event EventHandler<float>? FontSizeChanged;
    private float _FontSize = 10;
    public float FontSize
    {
        get => _FontSize;
        set
        {
            if (_FontSize == value) return;
            _FontSize = value;
            FontSizeChanged?.Invoke(this, _FontSize);
            Invalidate();
        }
    }
    public event EventHandler<FontStyle>? FontStyleChanged;
    private FontStyle _FontStyle = FontStyle.Regular;
    public FontStyle FontStyle
    {
        get => _FontStyle;
        set
        {
            if (_FontStyle == value) return;
            _FontStyle = value;
            FontStyleChanged?.Invoke(this, _FontStyle);
            Invalidate();
        }
    }
    public event EventHandler<int>? FontLetterSpacingChanged;
    private int _FontLetterSpacing = -2;
    public int FontLetterSpacing
    {
        get => _FontLetterSpacing;
        set
        {
            if (_FontLetterSpacing == value) return;
            _FontLetterSpacing = value;
            FontLetterSpacingChanged?.Invoke(this, _FontLetterSpacing);
            Invalidate();
        }
    }
    public event EventHandler<int>? TextPaddingChanged;
    private int _TextPaddingTop = 4;
    public int TextPaddingTop
    {
        get => _TextPaddingTop;
        set
        {
            if (_TextPaddingTop == value) return;
            _TextPaddingTop = value;
            TextPaddingChanged?.Invoke(this, _TextPaddingTop);
            Invalidate();
        }
    }
    public event EventHandler<int>? TextPaddingLeftChanged;
    private int _TextPaddingLeft = 2;
    public int TextPaddingLeft
    {
        get => _TextPaddingLeft;
        set
        {
            if (_TextPaddingLeft == value) return;
            _TextPaddingLeft = value;
            TextPaddingLeftChanged?.Invoke(this, _TextPaddingLeft);
            Invalidate();
        }
    }
    public event EventHandler<int>? TextPaddingBottomChanged;
    private int _TextPaddingBottom = 4;
    public int TextPaddingBottom
    {
        get => _TextPaddingBottom;
        set
        {
            if (_TextPaddingBottom == value) return;
            _TextPaddingBottom = value;
            TextPaddingBottomChanged?.Invoke(this, _TextPaddingBottom);
            Invalidate();
        }
    }
    public event EventHandler<int>? TextPaddingRightChanged;
    private int _TextPaddingRight = 10;
    public int TextPaddingRight
    {
        get => _TextPaddingRight;
        set
        {
            if (_TextPaddingRight == value) return;
            _TextPaddingRight = value;
            TextPaddingRightChanged?.Invoke(this, _TextPaddingRight);
            Invalidate();
        }
    }

    public int TextPadding
    {
        get => TextPaddingTop;
        set
        {
            //if (TextPaddingTop == value) return;
            TextPaddingTop = value;
            TextPaddingLeft = value;
            TextPaddingBottom = value;
            TextPaddingRight = value;
        }
    }
}
