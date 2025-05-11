using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Forms.Generic;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class MenuStripItem : ForeBorderBackControl
{
    private bool _Visible;
    private bool _IsOpen;
    GraphicsLayer Background;
    GraphicsLayer Foreground;

    public string Text { get; }

    public ObservableArrayCollection<MenuStripItem> Items { get; }

    public MenuStripItem(IApplicationForm applicationForm, Form? parentForm, Control? parentControl, string text)
        : base(applicationForm, parentForm, parentControl)
    {
        Text = text;
        Items = new ObservableArrayCollection<MenuStripItem>();
        Items.Added += (sender, item) => {
            item.Visible = false;
            Controls.Add(item);
        };
        Items.Removed += (sender, item) => {
            Controls.Remove(item);
        };

        Background = CanvasLayers.Create();
        Foreground = CanvasLayers.Create();
    }

    public override bool Visible { get => _Visible; set => _Visible = value; }

    public override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        if (ParentControl is MenuStrip)
        {
            Open();
        }
    }

    public override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        // Je zou hier een timer kunnen gebruiken om het menu niet meteen te sluiten
    }

    public override void OnMouseClick(MouseEventArgs e)
    {
        if (_IsOpen)
            Close();
        else
            Open();

        base.OnMouseClick(e);
    }

    private void Open()
    {
        _IsOpen = true;
        LayoutDropdownItems();
        foreach (var item in Items)
            item.Visible = true;
        Invalidate();
    }

    private void Close()
    {
        _IsOpen = false;
        foreach (var item in Items)
        {
            item.Visible = false;
            item.Close(); // recursive close
        }
        Invalidate();
    }

    private void LayoutDropdownItems()
    {
        int y = Height;
        foreach (var item in Items)
        {
            item.Left = 0;
            item.Top = y;
            item.Width = 100; //Math.Max(100, TextRenderer.MeasureText(item.Text, item.Font).Width + 20);
            item.Height = 24;
            y += item.Height;
        }
    }
    public override void OnDraw()
    {
        base.OnDraw();

        // Tekstmeting
        var meting = Foreground.MeasureText(Text, -1, -1, "Ebrima", 12, FontStyle.Regular, -2, ForeColor);

        // Horizontale centrering (optioneel: aanpassen afhankelijk van je layout engine)
        int textX = Left + 5; // Padding links
        int textY = Top + (Height - meting.Height) / 2;

        Foreground.DrawText(
            Text,
            textX,
            textY,
            -1,
            -1,
            "Ebrima",
            12,
            FontStyle.Regular,
            -2,
            ForeColor
        );
    }

}

