using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Controls.Generic;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls;

public class MenuStrip : ForeBorderBackControl
{
    public MenuStrip(IApplicationForm applicationForm)
        : base(applicationForm)
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
            x += item.Width;
        }
    }
}

