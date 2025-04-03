namespace VideoEditor.UI;

public partial class PropertiesControl : UserControl
{
    public PropertiesControl(Engine engine)
    {
        Engine = engine;
        Engine.PropertiesControl = this;
        InitializeComponent();
    }

    public Engine Engine { get; }

    internal void UpdateFps()
    {
        lblFps.Text = $"{Engine.FpsCounter.Fps} fps";
    }

    private void PropertiesControl_Resize(object sender, EventArgs e)
    {
        vScrollBar1.Top = 0;
        vScrollBar1.Left = ClientRectangle.Width - vScrollBar1.Width;
        vScrollBar1.Height = ClientRectangle.Height;
    }
}
