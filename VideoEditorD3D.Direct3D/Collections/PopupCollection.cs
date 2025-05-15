using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;
using Control = VideoEditorD3D.Direct3D.Controls.Control;
using Form = VideoEditorD3D.Direct3D.Forms.Form;

namespace VideoEditorD3D.Direct3D.Collections
{
    public class PopupCollection : ObservableArrayCollection<Popup>, IDisposable
    {
        public IApplicationForm ApplicationForm { get; }

        public PopupCollection(IApplicationForm applicationForm)
        {
            ApplicationForm = applicationForm;
            Added += OnChanged;
            Removed += OnRemoved;
        }

        public void OnChanged(object? sender, Popup item)
        {
        }

        public void OnRemoved(object? sender, Popup item)
        {
        }

        public Popup CreateNewPopup(int left, int top, int width, int height)
        {
            return new Popup(ApplicationForm)
            {
                Left = left,
                Top = top,
                Width = width,
                Height = height
            };
        }

        public void Dispose()
        {
            foreach (var popup in this)
            {
                popup.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
