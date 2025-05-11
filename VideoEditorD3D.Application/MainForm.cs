using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Application.Controls;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Application;

public class MainForm : Form
{
    private readonly DisplayControl DisplayControl;
    private readonly TimelineControl TimelineControl;
    private readonly PropertiesControl PropertiesControl;
    private readonly Label FpsLabel;

    public MainForm(ApplicationContext applicationContext, IApplicationForm applicationForm) : base(applicationForm)
    {
        BackColor = new RawColor4(0.125f, 0.25f, 0.5f, 1);

        DisplayControl = new DisplayControl(applicationContext, applicationForm, this, this);
        DisplayControl.BackColor = new RawColor4(0, 0, 0, 1);
        Controls.Add(DisplayControl);

        PropertiesControl = new PropertiesControl(applicationContext, applicationForm, this, this);
        PropertiesControl.BackColor = new RawColor4(0, 0, 0, 1);
        Controls.Add(PropertiesControl);

        TimelineControl = new TimelineControl(applicationContext, applicationForm, this, this);
        TimelineControl.BackColor = new RawColor4(0, 0, 0, 1);
        Controls.Add(TimelineControl);

        FpsLabel = new Label(applicationForm, this, this);
        FpsLabel.Left = 3;
        FpsLabel.Top = 3;
        FpsLabel.Width = 200;
        FpsLabel.Height = 20;
        FpsLabel.BackColor = new RawColor4(0, 0, 0, 0.5f);
        FpsLabel.BorderColor = new RawColor4(1, 1, 1, 1);
        FpsLabel.ForeColor = new RawColor4(1, 1, 1, 1);
        FpsLabel.BorderSize = 1;
        FpsLabel.FontSize = 6f;
        FpsLabel.Font = "Ebrima";
        FpsLabel.MouseDown += (sender, args) =>
        {
            FpsLabel.BackColor = new RawColor4(1, 1, 1, 1);
            FpsLabel.BorderColor = new RawColor4(0, 0, 0, 1);
            FpsLabel.ForeColor = new RawColor4(0, 0, 0, 1);
        };
        FpsLabel.MouseUp += (sender, args) =>
        {
            FpsLabel.BackColor = new RawColor4(0, 0, 0, 0.5f);
            FpsLabel.BorderColor = new RawColor4(1, 1, 1, 1);
            FpsLabel.ForeColor = new RawColor4(1, 1, 1, 1);
        };
        Controls.Add(FpsLabel);
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
        FpsLabel.Text = $"{ApplicationForm.Timers.FpsTimer.Fps}fps   {ApplicationForm.Timers.OnUpdateTimer.Time * 1000:F3}ms   {ApplicationForm.Timers.RenderToGpuTimer.Time * 1000:F3}ms";
        base.OnUpdate();
    }
}
