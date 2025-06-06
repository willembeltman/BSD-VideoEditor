using VideoEditorD3D.Direct3D.Interfaces;
using Control = VideoEditorD3D.Direct3D.Controls.Control;
using Form = VideoEditorD3D.Direct3D.Controls.Form;

namespace VideoEditorD3D.Direct3D.Collections
{
    public class FormCollection : ObservableArrayCollection<Form>, IDisposable
    {
        private readonly IApplicationForm ApplicationForm;

        public FormCollection(IApplicationForm applicationForm)
        {
            ApplicationForm = applicationForm;
            Added += OnAdded;
            Removed += OnRemoved;
        }

        public void OnAdded(object? sender, Form form)
        {
            Set(form);
        }

        private void Set(Control control)
        {
            control.ApplicationForm = ApplicationForm;

            foreach (var subcontrol in control.Controls)
            {
                Set(subcontrol);
            }
        }

        public void OnRemoved(object? sender, Form form)
        {
        }

        public void Dispose()
        {
            foreach (var form in this)
            {
                form.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
