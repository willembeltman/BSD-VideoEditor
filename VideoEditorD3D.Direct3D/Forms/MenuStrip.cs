using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Forms.Generic;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class MenuStrip : ForeBorderBackControl
{
    public MenuStrip(IApplicationForm applicationForm, Form? parentForm, Control? parentControl) : base(applicationForm, parentForm, parentControl)
    {
        Items = new ObservableArrayCollection<MenuStripItem>();
        Items.Added += (sender, item) => { Controls.Add(item); };
        Items.Removed += (sender, item) => { Controls.Remove(item); };
    }

    public ObservableArrayCollection<MenuStripItem> Items { get; }
}

public class MenuStripItem : ForeBorderBackControl
{
    private bool _Visible;

    public MenuStripItem(IApplicationForm applicationForm, Form? parentForm, Control? parentControl, string text) : base(applicationForm, parentForm, parentControl)
    {
        Text = text;
        Items = new ObservableArrayCollection<MenuStripItem>();
        Items.Added += (sender, item) => { Controls.Add(item); };
        Items.Removed += (sender, item) => { Controls.Remove(item); };
    }

    public string Text { get; }
    public override bool Visible { get => _Visible; set => _Visible = value; }

    public ObservableArrayCollection<MenuStripItem> Items { get; } = new ObservableArrayCollection<MenuStripItem>();
}
