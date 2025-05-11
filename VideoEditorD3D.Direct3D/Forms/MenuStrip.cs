using VideoEditorD3D.Direct3D.Forms.Generic;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms
{
    public class MenuStrip : ForeBorderBackControl
    {
        public MenuStrip(IApplicationForm application, Form? parentForm, Control? parentControl) : base(application, parentForm, parentControl)
        {
        }

        public List<MenuStripItem> Items { get; set; }
    }

    public class MenuStripItem : ForeBorderBackControl
    {
        public MenuStripItem(IApplicationForm application, Form? parentForm, Control? parentControl, string v) : base(application, parentForm, parentControl)
        {
        }

        public List<MenuStripItem> Items { get; set; }
    }
}
