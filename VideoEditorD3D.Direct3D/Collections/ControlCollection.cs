using Control = VideoEditorD3D.Direct3D.Controls.Control;
using Form = VideoEditorD3D.Direct3D.Forms.Form;

namespace VideoEditorD3D.Direct3D.Collections
{
    public class ControlCollection : ObservableArrayCollection<Control>, IDisposable
    {
        private readonly Control Control;
        private readonly Form Form;

        public ControlCollection(Control control)
        {
            Control = control;
            Form = control.ParentForm;
            Added += OnChanged;
            Removed += OnRemoved;
        }

        public ControlCollection(Form form)
        {
            Control = form;
            Form = form;
            Added += OnChanged;
            Removed += OnRemoved;
        }

        public void OnChanged(object? sender, Control item)
        {
            item.ParentForm = Form; 
            item.ParentControl = Control;
            Control.Invalidate();
        }

        public void OnRemoved(object? sender, Control item)
        {
            item.ParentForm = null;
            Control.Invalidate();
        }

        public void Dispose()
        {
            foreach (var control in this)
            {
                control.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
