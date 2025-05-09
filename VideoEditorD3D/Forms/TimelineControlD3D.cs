using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace VideoEditorD3D.Forms
{
    public class TimelineControlD3D : ControlD3D
    {
        public TimelineControlD3D(IApplication application, FormD3D? parentForm, ControlD3D? parentControl) : base(application, parentForm, parentControl)
        {
            Background = CreateCanvasLayer();
            Foreground = CreateCanvasLayer();
        }

        private readonly CanvasLayer Background;
        private readonly CanvasLayer Foreground;

        public override void OnDraw()
        {
            base.OnDraw();
        }
    }
}
