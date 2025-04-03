using VideoEditor.Types;

namespace VideoEditor.UI;

public partial class DisplayControl : UserControl
{
    public DisplayControl(Engine engine)
    {
        Engine = engine;
        Engine.DisplayControl = this;
        DirectXDisplayControl = new DisplayControlDX2D();
        Controls.Add(DirectXDisplayControl);
        DirectXDisplayControl.Dock = DockStyle.Fill;
        InitializeComponent();
    }

    public DisplayControlDX2D DirectXDisplayControl { get; }

    private void DisplayControl_Resize(object sender, EventArgs e)
    {
        var width = ClientRectangle.Width;
        var height = ClientRectangle.Height;

        var screenWidthBasedOnHeight = height * Engine.Timeline.Resolution.Width / Engine.Timeline.Resolution.Height;
        var screenHeightBasedOnWidth = width * Engine.Timeline.Resolution.Height / Engine.Timeline.Resolution.Width;

        if (height > screenHeightBasedOnWidth)
        {
            height = screenHeightBasedOnWidth;
        }
        if (width > screenWidthBasedOnHeight)
        {
            width = screenWidthBasedOnHeight;
        }

        var offsetX = (ClientRectangle.Width - width) / 2;
        var offsetY = (ClientRectangle.Height - height) / 2;

        DirectXDisplayControl.Top = offsetY;
        DirectXDisplayControl.Left = offsetX;
        DirectXDisplayControl.Width = width;
        DirectXDisplayControl.Height = height;
    }

    public void SetFrame(Frame frame)
    {
        DirectXDisplayControl.SetFrame(frame);
    }

    public Resolution Resolution => new Resolution(DirectXDisplayControl.Width, DirectXDisplayControl.Height);

    public Engine Engine { get; }
}
