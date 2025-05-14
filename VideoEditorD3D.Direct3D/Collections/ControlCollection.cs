using Control = VideoEditorD3D.Direct3D.Controls.Control;
using Form = VideoEditorD3D.Direct3D.Forms.Form;

namespace VideoEditorD3D.Direct3D.Collections
{
    public class ControlCollection : ObservableArrayCollection<Control>, IDisposable
    {
        private readonly Control Control;
        private readonly Form? Form;

        /// <summary>
        /// Initializes a collection of child controls for a parent control. 
        /// When a control is added, its <c>ParentControl</c> and <c>ParentForm</c> are automatically set,
        /// and the parent control is invalidated.
        /// </summary>
        /// <param name="control">The parent control that owns this collection.</param>
        public ControlCollection(Control control)
        {
            Control = control;
            Added += OnChanged;
            Removed += OnRemoved;
        }

        /// <summary>
        /// Initializes a collection of child controls for a form.
        /// Used internally to set the <c>ParentForm</c> reference of added controls directly,
        /// since forms have no parent forms.
        /// </summary>
        /// <param name="control">The parent control (usually the form itself).</param>
        /// <param name="form">
        /// The form to assign as <c>ParentForm</c> for added controls.
        /// This constructor is intended for internal use only.
        /// </param>
        internal ControlCollection(Control control, Form form)
        {
            Control = control;
            Form = form;
            Added += OnChanged;
            Removed += OnRemoved;
        }

        public void OnChanged(object? sender, Control item)
        {
            item.ParentForm = 
                Control.ParentForm // Which is null in case of a form.
                ?? Form; // So use the form passed in the constructor.
            item.ParentControl = Control;
            Control.Invalidate();
        }

        public void OnRemoved(object? sender, Control item)
        {
            item.ParentForm = null;
            item.ParentControl = null;
            Control.Invalidate();
        }

        public void Dispose()
        {
            foreach (var control in this)
            {
                control.Dispose();
            }
        }
    }
}
