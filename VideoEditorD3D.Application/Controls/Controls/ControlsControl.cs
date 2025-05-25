using VideoEditorD3D.Direct3D.Controls.Templates;

namespace VideoEditorD3D.Application.Controls.Controls
{
    public class ControlsControl : BackControl
    {
        private readonly PlayerControlsControl PlayerControlsControl;

        public ControlsControl()
        {
            Height = 38;
            BackColor = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 0.5f);
            PlayerControlsControl = new PlayerControlsControl();
            Controls.Add(PlayerControlsControl);
            Resize += ControlsControl_Resize;
        }

        private void ControlsControl_Resize(object? sender, EventArgs e)
        {
            PlayerControlsControl.Width = 36 * 3 + 10;
            PlayerControlsControl.Height = Height;
        }

    }
}
