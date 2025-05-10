using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D;

namespace VideoEditorD3D.Application.Controls
{
    public class PropertiesControl : Control
    {
        public PropertiesControl(ApplicationContext application, IApplicationForm applicationForm, Form? parentForm, Control? parentControl) : base(applicationForm, parentForm, parentControl)
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
