using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Forms.Generic;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class MenuStrip : ForeBorderBackControl
{
    public MenuStrip(IApplicationForm applicationForm, Form? parentForm, Control? parentControl)
        : base(applicationForm, parentForm, parentControl)
    {
        Items = new ObservableArrayCollection<MenuStripItem>();
        Items.Added += (sender, item) => {
            Controls.Add(item);
            PerformLayout();
        };
        Items.Removed += (sender, item) => {
            Controls.Remove(item);
            PerformLayout();
        };
        Height = 24;
    }

    public ObservableArrayCollection<MenuStripItem> Items { get; }

    public void PerformLayout()
    {
        int x = 0;
        foreach (var item in Items)
        {
            item.Left = x;
            item.Top = 0;
            item.Height = Height;
            item.Width = 60; // Math.Max(60, TextRenderer.MeasureText(item.Text, item.Font).Width + 20);
            x += item.Width;
        }
    }
}

