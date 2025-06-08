using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Controls.Templates;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Events;
using VideoEditorD3D.Direct3D.Interfaces;
using Form = VideoEditorD3D.Direct3D.Controls.Form;

namespace VideoEditorD3D.Direct3D.Controls;

public class MenuStripItem : ForeBorderBackControl
{
    private readonly Form Popup;
    private readonly GraphicsLayer Foreground;
    public readonly string Text;

    public Action? OnClick { get; }

    public event EventHandler<MouseEvent>? Clicked;
    public ObservableArrayCollection<MenuStripItem> Items { get; }
    public MenuStripItem? ParentItem { get; private set; }

#nullable disable
    public MenuStrip MenuStrip { get; internal set; }
    public bool Opened { get; private set; }
#nullable enable

    public MenuStripItem(string text, Action? onClick = null)
    {
        Text = text;
        OnClick = onClick;

        Foreground = GraphicsLayers.CreateNewLayer();

        LostFocus += MenuStripItem_LostFocus;
        Load += MenuStripItem_Load;
        Click += MenuStripItem_MouseClick;
        MouseEnter += MenuStripItem_MouseEnter;
        MouseLeave += MenuStripItem_MouseLeave;
        Draw += MenuStripItem_Draw;

        Popup = new Form();

        Items = new ObservableArrayCollection<MenuStripItem>();
        Items.Added += (sender, item) =>
        {
            item.ParentItem = this;
            item.MenuStrip = MenuStrip;
            Popup.Controls.Add(item);
            item.Resize += (sender, item) => { PerformLayout(); };
        };
        Items.Removed += (sender, item) =>
        {
            Popup.Controls.Remove(item);
        };

        Top = 42;
        Height = 42;
        Width = 100;
    }


    private void MenuStripItem_Load(object? sender, EventArgs e)
    {
        Popup.BackColor = MenuStrip.MenuBackColor;

        ForeColor = MenuStrip.NormalForeColor;
        BackColor = MenuStrip.NormalBackColor;
        BorderColor = MenuStrip.NormalBorderColor;
        BorderSize = MenuStrip.NormalBorderSize;

        Font = MenuStrip.Font;
        FontSize = MenuStrip.FontSize;
        FontLetterSpacing = MenuStrip.FontLetterSpacing;
        FontStyle = MenuStrip.FontStyle;
        TextPaddingLeft = MenuStrip.TextPaddingLeft;
        TextPaddingTop = MenuStrip.TextPaddingTop;
        TextPaddingRight = MenuStrip.TextPaddingRight;
        TextPaddingBottom = MenuStrip.TextPaddingBottom;

        MeasureSize();
    }

    private void MenuStripItem_LostFocus(object? sender, MouseEvent e)
    {
        MenuStrip.CloseAll();
    }

    private void MenuStripItem_MouseClick(object? sender, MouseEvent e)
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
            OnClick?.Invoke();
        }
    }
    private void MenuStripItem_MouseEnter(object? sender, EventArgs e)
    {
        ForeColor = MenuStrip.MouseOverForeColor;
        BackColor = MenuStrip.MouseOverBackColor;
        BorderColor = MenuStrip.MouseOverBorderColor;
        BorderSize = MenuStrip.MouseOverBorderSize;

        if (ParentItem == null && MenuStrip.Opened)
        {
            MenuStrip.CloseAll();
            Open();
        }
    }
    private void MenuStripItem_MouseLeave(object? sender, EventArgs e)
    {
        ForeColor = MenuStrip.NormalForeColor;
        BackColor = MenuStrip.NormalBackColor;
        BorderColor = MenuStrip.NormalBorderColor;
        BorderSize = MenuStrip.NormalBorderSize;
    }

    private void MenuStripItem_Draw(object? sender, EventArgs e)
    {
        int textX = TextPaddingLeft; // Padding links
        int textY = TextPaddingTop;

        Foreground.StartDrawing();
        Foreground.DrawText(Text, textX, textY, -1, -1, Font, FontSize, FontStyle, FontLetterSpacing, ForeColor);
        Foreground.EndDrawing();
    }

    public void PerformLayout()
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
        Invalidate();
    }
    private void MeasureSize()
    {
        var meting = Foreground.MeasureText(Text, -1, -1, Font, FontSize, FontStyle, FontLetterSpacing, ForeColor);
        Width = meting.Width + TextPaddingLeft + TextPaddingRight;
        Height = meting.Height + TextPaddingTop + TextPaddingBottom;
    }

    public void Open()
    {
        if (ParentItem != null)
            ParentItem.Opened = true;
        MenuStrip.Opened = true;
        ApplicationForm.Forms.Add(Popup);
    }
    public void Close()
    {
        ApplicationForm.Forms.Remove(Popup);
        if (ParentItem != null)
            ParentItem.Opened = false;
    }

    public void InvalidateAllChildren()
    {
        Popup.Invalidate();
        Invalidate();
        foreach (var item in Items)
            item.InvalidateAllChildren();
    }
}

