using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Controls;

namespace VideoEditorD3D.Application.Controls;

public class FpsControl : Label
{
    public FpsControl()
    {
        BackColor = new RawColor4(0, 0, 0, 0.5f);
        BorderColor = new RawColor4(1, 1, 1, 1);
        ForeColor = new RawColor4(1, 1, 1, 1);
        BorderSize = 1;
        TextPadding = 2;
        TextPaddingRight = 5;
        FontSize = 7f;
        Font = "Ebrima";
        Update += FpsControl_Update;
    }

    private void FpsControl_Update(object? sender, EventArgs e)
    {
        Text = $"{ApplicationForm.Timers.FpsTimer.Fps}fps   {ApplicationForm.Timers.OnUpdateTimer.Time * 1000:F3}ms   {ApplicationForm.Timers.RenderToGpuTimer.Time * 1000:F3}ms";

        //var size = Foreground.MeasureText(Text, -1, -1, Font, FontSize, FontStyle, FontLetterSpacing, ForeColor);
        //Width = TextPaddingLeft + TextPaddingRight + size.Width;
        //Height = TextPaddingTop + TextPaddingBottom + size.Height;
    }
}
