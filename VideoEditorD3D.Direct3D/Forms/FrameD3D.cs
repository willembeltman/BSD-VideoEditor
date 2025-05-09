using VideoEditorD3D.Types;

namespace VideoEditorD3D.Direct3D.Forms
{
    public class FrameD3D : ControlD3D
    {
        public FrameD3D(FormD3D? parentForm) : base(parentForm)
        {
            Foreground = CreateCanvasLayer();
        }

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
            Foreground.Clear();
            if (Frame == null) return;
            Foreground.DrawFrame(Left, Top, Width, Height, Frame);
        }
    }
}
