using Control = VideoEditorD3D.Direct3D.Forms.Control;

namespace VideoEditorD3D.Direct3D.Collections
{
    public class ControlCollection : ObservableArrayCollection<Control>
    {
        private Control Control;

        public ControlCollection(Control control)
        {
            Control = control;
            Changed += (sender, args) =>
            {
                Control.Invalidate();
            };
        }
    }
}
