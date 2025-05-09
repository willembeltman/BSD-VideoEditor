using VideoEditorD3D.Direct3D;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Types;

namespace VideoEditorD3D.Forms
{
    public class DisplayControlD3D : ControlD3D
    {
        public DisplayControlD3D(IApplication application, FormD3D? parentForm, ControlD3D? parentControl) : base(application, parentForm, parentControl)
        {
            Background = CreateCanvasLayer();
            Foreground = CreateCanvasLayer();
        }

        private readonly CanvasLayer Background;

        private readonly CanvasLayer Foreground;

        private Frame? _Frame;
        public virtual Frame? Frame
        {
            get => _Frame;
            set
            {
                if (_Frame != value)
                {
                    _Frame = value;
                    Invalidate();
                }
            }
        }

        public override void OnDraw()
        {
            Background.StartDrawing();
            Foreground.StartDrawing();

            Background.FillRectangle(Left, Top, Width, Height, BackgroundColor);

            if (Frame != null)
                Foreground.DrawFrame(Left, Top, Width, Height, Frame);

            Background.EndDrawing();
            Foreground.EndDrawing();
        }
    }
}
