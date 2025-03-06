using VideoEditor.Static;

namespace VideoEditor.UI;

public partial class PropertiesControl : UserControl
{
    public PropertiesControl()
    {
        InitializeComponent();
        Engine.PropertiesControl = this;
    }

    private void PropertiesControl_Resize(object sender, EventArgs e)
    {
        vScrollBar1.Top = 0;
        vScrollBar1.Left = ClientRectangle.Width - vScrollBar1.Width;
        vScrollBar1.Height = ClientRectangle.Height;
    }
}
