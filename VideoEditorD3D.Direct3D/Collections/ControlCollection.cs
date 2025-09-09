using VideoEditorD3D.Direct3D.Interfaces;
using Control = VideoEditorD3D.Direct3D.Controls.Control;
using Form = VideoEditorD3D.Direct3D.Controls.Form;

namespace VideoEditorD3D.Direct3D.Collections
{
    public class ControlCollection : ObservableArrayCollection<Control>, IDisposable
    {
        private readonly Control ParentControl;

        public ControlCollection(Control control)
        {
            ParentControl = control;
            Added += OnAdded;
            Removed += OnRemoved;
        }

        //public ControlCollection(Form form)
        //{
        //    ParentControl = form;
        //    ParentForm = form;
        //    Added += OnAdded;
        //    Removed += OnRemoved;
        //}

        public void OnAdded(object? sender, Control item)
        {
            item.ParentControl = ParentControl;
            Set(item);
            ParentControl.Invalidate();
        }
        private void Set(Control control)
        {
            control.ParentForm = ParentControl.ParentForm;
            control.ApplicationForm = ParentControl.ApplicationForm;

            foreach (var subcontrol in control.Controls)
            {
                subcontrol.ParentControl = control;
                Set(subcontrol);
            }
        }

        public void OnRemoved(object? sender, Control item)
        {
            ParentControl.Invalidate();
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
