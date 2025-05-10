using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Helpers;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D
{
    public class ApplicationFormEventHandlers
    {
        private IApplicationForm applicationForm;
        private IApplication application;
        private FormD3D CurrentForm => applicationForm.CurrentForm;

        public ApplicationFormEventHandlers(IApplicationForm applicationForm, IApplication application)
        {
            this.applicationForm = applicationForm;
            this.application = application;
        }

        public void OnKeyPress(object? sender, KeyPressEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnKeyPress(e);
        }
        public void OnKeyUp(object? sender, KeyEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnKeyUp(e);
        }
        public void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnKeyDown(e);
        }
        public void OnMouseClick(object? sender, MouseEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnMouseClick(e);
        }
        public void OnMouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnMouseDoubleClick(e);
        }
        public void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnMouseUp(e);
        }
        public void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnMouseDown(e);
        }
        public void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnMouseMove(e);
        }
        public void OnMouseWheel(object? sender, MouseEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnMouseWheel(e);
        }
        public void OnDragDrop(object? sender, DragEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnDragDrop(e);
        }
        public void OnDragEnter(object? sender, DragEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnDragEnter(e);
        }
        public void OnDragOver(object? sender, DragEventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnDragOver(e);
        }
        public void OnDragLeave(object? sender, EventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnDragLeave(e);
        }
    }
}