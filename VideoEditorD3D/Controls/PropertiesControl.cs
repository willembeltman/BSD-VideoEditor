using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D;

namespace VideoEditorD3D.Controls
{
    public class PropertiesControl : Direct3D.Forms.Control
    {
        public PropertiesControl(Application application, IApplicationForm applicationForm, Direct3D.Forms.Form? parentForm, Direct3D.Forms.Control? parentControl) : base(applicationForm, parentForm, parentControl)
        {
            Background = CreateCanvasLayer();
            Foreground = CreateCanvasLayer();
        }

        private readonly GraphicsLayer Background;
        private readonly GraphicsLayer Foreground;


        private RawColor4 _BackgroundColor;
        public RawColor4 BackgroundColor
        {
            get => _BackgroundColor;
            set
            {
                _BackgroundColor = value;
                Invalidate();
            }
        }

        public override void OnDraw()
        {
            Background.StartDrawing();
            Background.FillRectangle(Left, Top, Width, Height, BackgroundColor);
            Background.EndDrawing();
        }
    }
}
