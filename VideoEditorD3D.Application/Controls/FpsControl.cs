using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Application.Controls;

public class FpsControl : Label
{
    public FpsControl(IApplicationForm application) : base(application)
    {
        BackColor = new RawColor4(0, 0, 0, 0.5f);
        BorderColor = new RawColor4(1, 1, 1, 1);
        ForeColor = new RawColor4(1, 1, 1, 1);
        BorderSize = 1;
        FontSize = 6f;
        Font = "Ebrima";
    }

    public override void OnUpdate()
    {
        Text = $"{ApplicationForm.Timers.FpsTimer.Fps}fps   {ApplicationForm.Timers.OnUpdateTimer.Time * 1000:F3}ms   {ApplicationForm.Timers.RenderToGpuTimer.Time * 1000:F3}ms";
        base.OnUpdate();
    }
}
