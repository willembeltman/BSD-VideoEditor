using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Forms;

namespace VideoEditorD3D.Forms
{
    public class MainFormD3D : FormD3D
    {
        public DisplayControlD3D Frame { get; }
        public List<ButtonD3D> Buttons { get; }
        public override IApplication Application { get; }

        public MainFormD3D(IApplication application) : base(application)
        {
            Application = application;
            Frame = new DisplayControlD3D(application, this, this)
            {
                BackgroundColor = new RawColor4(0, 0, 1, 1)
            };
            AddControl(Frame);

            Buttons = [];
            for (var i = 0; i < 16; i++)
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

            var y = 10d;
            var h = Convert.ToDouble(Height - 20 - Buttons.Count * 3) / Buttons.Count;

            foreach (var button in Buttons)
            {
                button.Top = Convert.ToInt32(y);
                button.Left = 10;
                button.Width = Width - 200;
                button.Height = Convert.ToInt32(h);
                y += h + 3;
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
