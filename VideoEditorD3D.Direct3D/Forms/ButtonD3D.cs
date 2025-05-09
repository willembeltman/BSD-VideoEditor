using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms
{
    public class ButtonD3D : ControlD3D
    {
        public ButtonD3D(IApplication application, FormD3D? parentForm, ControlD3D? parentControl) : base(application, parentForm, parentControl)
        {
            Background = CreateCanvasLayer();
            Foreground = CreateCanvasLayer();
        }

        private readonly CanvasLayer Background;
        private readonly CanvasLayer Foreground;

        private string _Text = string.Empty;
        public string Text
        {
            get => _Text;
            set
            {
                if (_Text != value)
                {
                    _Text = value;
                    Invalidate();
                }
            }
        }

        private RawColor4 _ForegroundColor;
        public RawColor4 ForegroundColor
        {
            get => _ForegroundColor;
            set
            {
                _ForegroundColor = value;
                Invalidate();
            }
        }

        private string _Font = "Arial";
        public string Font
        {
            get => _Font;
            set
            {
                _Font = value;
                Invalidate();
            }
        }

        private float _FontSize = 10;
        public float FontSize
        {
            get => _FontSize;
            set
            {
                _FontSize = value;
                Invalidate();
            }
        }

        public override void OnDraw()
        {
            Background.StartDrawing();
            Foreground.StartDrawing();

            Background.FillRectangle(Left, Top, Width, Height, BackgroundColor);
            Foreground.DrawText(Text, Left, Top, Font, FontSize, ForegroundColor, BackgroundColor);

            Background.EndDrawing();
            Foreground.EndDrawing();
        }
    }
}
