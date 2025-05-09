using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Controls;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using Color = SharpDX.Color;

namespace VideoEditorD3D.Forms
{
    public class MainForm : FormD3D
    {
        private readonly DisplayControl DisplayControl;
        private readonly TimelineControl TimelineControl;
        private readonly PropertiesControl PropertiesControl;
        private readonly ButtonD3D Button;

        public MainForm(Application application, IApplicationForm applicationForm) : base(applicationForm)
        {
            BackgroundColor = new Color(64, 96, 255, 32);

            DisplayControl = new DisplayControl(application, applicationForm, this, this)
            {
                BackgroundColor = new RawColor4(0, 0, 0, 1)
            };
            AddControl(DisplayControl);

            PropertiesControl = new PropertiesControl(application, applicationForm, this, this)
            {
                BackgroundColor = new RawColor4(0, 0, 0, 1)
            };
            AddControl(PropertiesControl);

            TimelineControl = new TimelineControl(application, applicationForm, this, this)
            {
                BackgroundColor = new RawColor4(0, 0, 0, 1)
            };
            AddControl(TimelineControl);

            Button = new ButtonD3D(applicationForm, this, this)
            {
                ForegroundColor = new RawColor4(1, 1, 1, 1),
                BackgroundColor = new RawColor4(1, 0, 0, 1),
                FontSize = 12
            };
            AddControl(Button);
        }

        public override void OnResize()
        {
            Button.Top = 10;
            Button.Left = 10;
            Button.Width = 200;
            Button.Height = 32;

            var marge = 10;
            var propertiesWidth = 240;
            var timelineHeight = 240;

            DisplayControl.Top = marge;
            DisplayControl.Left = marge;
            DisplayControl.Width = Width - propertiesWidth - marge * 3;
            DisplayControl.Height = Height - timelineHeight - marge * 3;

            PropertiesControl.Top = marge;
            PropertiesControl.Left = DisplayControl.Right + marge;
            PropertiesControl.Width = propertiesWidth;
            PropertiesControl.Height = Height - timelineHeight - marge * 3;

            TimelineControl.Top = DisplayControl.Bottom + marge;
            TimelineControl.Left = marge;
            TimelineControl.Width = Width - marge * 2;
            TimelineControl.Height = timelineHeight;

            base.OnResize();
        }

        public override void OnUpdate()
        {
            Button.Text = $"{ApplicationForm.Timers.FpsTimer.Fps}fps  {ApplicationForm.Timers.OnUpdateTimer.Time * 1000:F3}ms  {ApplicationForm.Timers.DrawTimer.Time * 1000:F3}ms";
            base.OnUpdate();
        }
    }
}
