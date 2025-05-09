using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms;

namespace VideoEditorD3D.Forms
{
    public class MainForm : FormD3D
    {
        public MainForm(IApplication application) : base(application)
        {
            Button = new ButtonD3D(this)
            {
                Text = "Hello world",
                ForegroundColor = new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1),
                BackgroundColor = new SharpDX.Mathematics.Interop.RawColor4(1, 0, 0, 1)
            };
            AddControl(Button);

            Frame = new FrameD3D(this);
            AddControl(Frame);
        }

        public ButtonD3D Button { get; }
        public FrameD3D Frame { get; }

        public override void OnResize()
        {
            Button.Top = 10;
            Button.Left = 10;
            Button.Width = Width - 20;
            Button.Height = 30;

            Frame.Top = 50;
            Frame.Left = 10;
            Frame.Width = Width - 20;
            Frame.Height = Height - 60;

            base.OnResize();
        }
    }
}
