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
            PlayerControlsControl.Resize += PlayerControlsControl_Resize;
        }

        private void PlayerControlsControl_Resize(object? sender, EventArgs e)
        {
            Height = PlayerControlsControl.Height;
        }

    }
}
