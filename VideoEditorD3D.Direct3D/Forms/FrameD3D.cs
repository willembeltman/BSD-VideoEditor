using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Types;

namespace VideoEditorD3D.Direct3D.Forms
{
    public class FrameD3D : ControlD3D
    {
        public FrameD3D(IApplicationD3D application, FormD3D? parentForm, ControlD3D? parentControl) : base(application, parentForm, parentControl)
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

        public override void Draw()
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
