using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Engine.Controls;

namespace VideoEditorD3D.Engine.Forms
{
    public class MainForm : Form
    {
        private readonly DisplayControl DisplayControl;
        private readonly TimelineControl TimelineControl;
        private readonly PropertiesControl PropertiesControl;
        private readonly Label FpsLabel;

        public MainForm(Application application, IApplicationForm applicationForm) : base(applicationForm)
        {
            BackgroundColor = new RawColor4(0.125f, 0.25f, 0.5f, 1);

            DisplayControl = new DisplayControl(application, applicationForm, this, this);
            DisplayControl.BackgroundColor = new RawColor4(0, 0, 0, 1);
            AddControl(DisplayControl);

            PropertiesControl = new PropertiesControl(application, applicationForm, this, this);
            PropertiesControl.BackgroundColor = new RawColor4(0, 0, 0, 1);
            AddControl(PropertiesControl);

            TimelineControl = new TimelineControl(application, applicationForm, this, this);
            TimelineControl.BackgroundColor = new RawColor4(0, 0, 0, 1);
            AddControl(TimelineControl);

            FpsLabel = new Label(applicationForm, this, this);
            FpsLabel.Left = 3;
            FpsLabel.Top = 3;
            FpsLabel.Width = 200;
            FpsLabel.Height = 20;
            FpsLabel.BackgroundColor = new RawColor4(0, 0, 0, 0.5f);
            FpsLabel.BorderSize = 1;
            FpsLabel.FontSize = 6f;
            FpsLabel.Font = "Ebrima";
            AddControl(FpsLabel);
        }

        public override void OnResize()
        {
            var marge = 10;
            var propertiesWidth = 240;
            var timelineHeight = 240;

            DisplayControl.Top = marge;
            DisplayControl.Left = marge;
            DisplayControl.Width = Width - propertiesWidth - marge * 3;
            DisplayControl.Height = Height - timelineHeight - marge * 3;

            PropertiesControl.Top = marge;
            PropertiesControl.Left = marge + Width - propertiesWidth - marge * 3 + marge;
            PropertiesControl.Width = propertiesWidth;
            PropertiesControl.Height = Height - timelineHeight - marge * 3;

            TimelineControl.Top = marge + Height - timelineHeight - marge * 3 + marge;
            TimelineControl.Left = marge;
            TimelineControl.Width = Width - marge * 2;
            TimelineControl.Height = timelineHeight;

            base.OnResize();
        }

        public override void OnUpdate()
        {
            FpsLabel.Text = $"{ApplicationForm.Timers.FpsTimer.Fps}fps   {ApplicationForm.Timers.OnUpdateTimer.Time * 1000:F3}ms   {ApplicationForm.Timers.DrawTimer.Time * 1000:F3}ms";
            base.OnUpdate();
        }
    }
}
