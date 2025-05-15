using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Controls.Templates;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls;

public class MenuStrip : BorderBackControl
{
    public MenuStrip(IApplicationForm applicationForm)
        : base(applicationForm)
    {
        Items = new ObservableArrayCollection<MenuStripItem>();
        Items.Added += (sender, item) =>
        {
            item.MenuStrip = this;
            Controls.Add(item);
            CheckSize();
            item.Resize += (sender, item) => { CheckSize(); };
        };
        Items.Removed += (sender, item) => {
            Controls.Remove(item);
            CheckSize();
        };
        BorderColor = new SharpDX.Mathematics.Interop.RawColor4(1, 0, 0, 1);
    }

    private void CheckSize()
    {
        Width = Items.Sum(a => a.Width);
        Height = Items.Max(a => a.Height);
    }



    public ObservableArrayCollection<MenuStripItem> Items { get; }

    public override void OnResize()
    {
        base.OnResize();

        int x = 0;
        foreach (var item in Items)
        {
            item.Left = x;
            item.Top = 0;
            x += item.Width;
        }
    }

    public void CloseAll()
    {
        foreach (var item in Items)
        {
            item.Close();
        }
    }
}

