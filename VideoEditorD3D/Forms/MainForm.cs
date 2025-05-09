using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms;

namespace VideoEditorD3D.Forms
{
    public class MainForm : FormD3D
    {
        public FrameD3D Frame { get; }
        public List<ButtonD3D> Buttons { get; }

        public MainForm(IApplication application) : base(application)
        {
            Frame = new FrameD3D(application, this, this)
            {
                BackgroundColor = new RawColor4(0, 0, 1, 1)
            };
            AddControl(Frame);

            
            Buttons = new List<ButtonD3D>();
            for (var i = 0; i < 32; i++)
            {
                var button = new ButtonD3D(application, this, this)
                {
                    ForegroundColor = new RawColor4(1, 1, 1, 1),
                    BackgroundColor = new RawColor4(1, 0, 0, 1)
                };
                Buttons.Add(button);
                AddControl(button);
            }
        }

        public override void OnResize()
        {
            Frame.Top = 10;
            Frame.Left = 10;
            Frame.Width = Width - 20;
            Frame.Height = Height - 20;

            var y = 10;
            var h = Height / Buttons.Count;

            foreach (var button in Buttons)
            {
                button.Top = y;
                button.Left = 10;
                button.Width = Width - 20;
                button.Height = h;
                y += h;
            }

            base.OnResize();
        }
        public override void OnUpdate()
        {
            foreach (var button in Buttons)
            {
                button.Text = $"{Application.Timers.FpsTimer.Fps}fps  {Application.Timers.OnUpdateTimer.Time * 1000:F3}ms  {Application.Timers.DrawTimer.Time * 1000:F3}ms";
            }
            base.OnUpdate();
        }
    }
}
