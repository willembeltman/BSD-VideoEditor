using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms;
using FormD3D = VideoEditorD3D.Direct3D.Forms.FormD3D;

namespace VideoEditorD3D.Forms
{
    public class MainForm : FormD3D
    {
        public MainForm(IApplication application) : base(application)
        {
            Button = new ButtonD3D(this)
            {
                Top = 10,
                Left = 10,
                Width = 10,
                Height = 10,
                ForegroundColor = new SharpDX.Mathematics.Interop.RawColor4(1, 1, 1, 1),
                BackgroundColor = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 1)
            };
            AddControl(Button);
        }

        public ButtonD3D Button { get; }

        public override void Draw()
        {
        }
    }
}
