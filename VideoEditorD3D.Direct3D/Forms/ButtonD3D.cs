using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D.Forms
{
    public class ButtonD3D : ControlD3D
    {
        public ButtonD3D(FormD3D? parentForm) : base(parentForm)
        {
            Background = CreateCanvasLayer();
            Borders = CreateCanvasLayer();
            Foreground = CreateCanvasLayer();
        }

        private readonly CanvasLayer Background;
        private readonly CanvasLayer Borders;
        private readonly CanvasLayer Foreground;

        public override void Draw()
        {
            Background.FillRectangle(Left, Top, Width, Height, BackgroundColor);
        }
    }
}
