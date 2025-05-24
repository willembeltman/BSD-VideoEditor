using VideoEditorD3D.Direct3D.Controls.Templates;

namespace VideoEditorD3D.Application.Controls.Controls
{
    public class ControlsControl : BackControl
    {
        public ControlsControl()
        {
            Height = 36;
            BackColor = new SharpDX.Mathematics.Interop.RawColor4(0, 0, 0, 0.5f);
        }
    }
}
