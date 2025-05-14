using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls.Generic;

public class ForeBorderBackControl(IApplicationForm application) : BorderBackControl(application)
{
    private string _Font = "Ebrima";
    private float _FontSize = 10;
    private int _FontLetterSpacing = -2;
    private FontStyle _FontStyle = FontStyle.Regular;
    private RawColor4 _ForeColor = new RawColor4(0, 0, 0, 0);

    public RawColor4 ForeColor
    {
        get => _ForeColor;
        set
        {
            if (_ForeColor.Equals(value)) return;
            _ForeColor = value;
            Invalidate();
        }
    }
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
}
