using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Controls.Templates;
using VideoEditorD3D.Direct3D.Forms;

namespace VideoEditorD3D.Direct3D.Controls;

public class MenuStripItem : ForeBorderBackControl
{

    public event EventHandler<MouseEventArgs>? Clicked;


    public ObservableArrayCollection<MenuStripItem> Items { get; }
    Popup Popup { get; }
    public string Text { get; }
    protected GraphicsLayer Foreground { get; }
#nullable disable
    public MenuStrip MenuStrip { get; internal set; }
#nullable enable

    public MenuStripItem(IApplicationForm applicationForm, string text) : base(applicationForm)
    {
        Text = text;

        Foreground = GraphicsLayers.CreateNewLayer();

        BackColor = new SharpDX.Mathematics.Interop.RawColor4(0.5f, 0.5f, 0.5f, 1);
        ForeColor = new SharpDX.Mathematics.Interop.RawColor4(1f, 1f, 1f, 1f);
        MeasureSize();

        FontSizeChanged += MenuStripItem_FontSizeChanged;
        FontChanged += MenuStripItem_FontChanged;
        FontLetterSpacingChanged += MenuStripItem_FontLetterSpacingChanged;
        FontStyleChanged += MenuStripItem_FontStyleChanged;
        GotFocus += MenuStripItem_GotFocus;
        LostFocus += MenuStripItem_LostFocus;

        Popup = new Popup(ApplicationForm);

        Items = new ObservableArrayCollection<MenuStripItem>();
        Items.Added += (sender, item) =>
        {
            item.MenuStrip = MenuStrip;
            Popup.Controls.Add(item);
            PerformLayout();
        };
        Items.Removed += (sender, item) =>
        {
            Popup.Controls.Remove(item);
            PerformLayout();
        };
    }

    private void PerformLayout()
    {
        Popup.Width = Items.Max(a => a.Width);
        Popup.Height = Items.Sum(a => a.Height);
        Popup.Left = AbsoluteLeft;
        Popup.Top = AbsoluteBottom;
        var top = 0;
        foreach (var item in Items)
        {
            item.Top = top;
            item.Width = Popup.Width;
            top += item.Height;
        }

    }

    private void MenuStripItem_FontStyleChanged(object? sender, FontStyle e) => MeasureSize();
    private void MenuStripItem_FontLetterSpacingChanged(object? sender, int e) => MeasureSize();
    private void MenuStripItem_FontChanged(object? sender, string e) => MeasureSize();
    private void MenuStripItem_FontSizeChanged(object? sender, float e) => MeasureSize();

    private void MenuStripItem_GotFocus(object? sender, MouseEventArgs e)
    {
    }
    private void MenuStripItem_LostFocus(object? sender, MouseEventArgs e)
    {
        MenuStrip.CloseAll();
    }


    private void MeasureSize()
    {
        var meting = Foreground.MeasureText(Text, -1, -1, Font, FontSize, FontStyle, FontLetterSpacing, ForeColor);
        Width = meting.Width + TextPaddingLeft + TextPaddingRight;
        Height = meting.Height + TextPaddingTop + TextPaddingBottom;
    }

    public override bool OnMouseClick(MouseEventArgs e)
    {
        if (ParentControl is MenuStrip)
        {
            (ParentControl as MenuStrip)!.CloseAll();
        }

        if (Items.Count > 0)
        {
            Open();
        }
        else
        {
            Clicked?.Invoke(this, e);
        }

        return base.OnMouseClick(e);
    }
    public override void OnMouseEnter(EventArgs e)
    {
        var temp = ForeColor;
        ForeColor = BackColor;
        BackColor = temp;
        base.OnMouseEnter(e);
    }
    public override void OnMouseLeave(EventArgs e)
    {
        var temp = ForeColor;
        ForeColor = BackColor;
        BackColor = temp;
        base.OnMouseLeave(e);
    }

    public void Open()
    {
        ApplicationForm.Popups.Add(Popup);
    }
    public void Close()
    {
        ApplicationForm.Popups.Remove(Popup);
    }


    public override void OnDraw()
    {
        base.OnDraw();

        int textX = TextPaddingLeft; // Padding links
        int textY = TextPaddingTop;

        Foreground.StartDrawing();
        Foreground.DrawText(Text, textX, textY, -1, -1, Font, FontSize, FontStyle, FontLetterSpacing, ForeColor);
        Foreground.EndDrawing();
    }

}

