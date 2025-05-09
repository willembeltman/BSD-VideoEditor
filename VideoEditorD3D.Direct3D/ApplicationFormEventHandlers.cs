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
        private WindowsScaling WindowsScaling => applicationForm.WindowsScaling;

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
            if (WindowsScaling == null || CurrentForm == null) return;
            var newX = e.X * WindowsScaling.Scaling;
            var newY = e.Y * WindowsScaling.Scaling;
            CurrentForm.OnMouseClick(e, new RawVector2(newX, newY));
        }
        public void OnMouseDoubleClick(object? sender, MouseEventArgs e)
        {
            if (WindowsScaling == null || CurrentForm == null) return;
            var newX = e.X * WindowsScaling.Scaling;
            var newY = e.Y * WindowsScaling.Scaling;
            CurrentForm.OnMouseDoubleClick(e, new RawVector2(newX, newY));
        }
        public void OnMouseUp(object? sender, MouseEventArgs e)
        {
            if (WindowsScaling == null || CurrentForm == null) return;
            var newX = e.X * WindowsScaling.Scaling;
            var newY = e.Y * WindowsScaling.Scaling;
            CurrentForm.OnMouseUp(e, new RawVector2(newX, newY));
        }
        public void OnMouseDown(object? sender, MouseEventArgs e)
        {
            if (WindowsScaling == null || CurrentForm == null) return;
            var newX = e.X * WindowsScaling.Scaling;
            var newY = e.Y * WindowsScaling.Scaling;
            CurrentForm.OnMouseDown(e, new RawVector2(newX, newY));
        }
        public void OnMouseMove(object? sender, MouseEventArgs e)
        {
            if (WindowsScaling == null || CurrentForm == null) return;
            var newX = e.X * WindowsScaling.Scaling;
            var newY = e.Y * WindowsScaling.Scaling;
            CurrentForm.OnMouseMove(e, new RawVector2(newX, newY));
        }
        public void OnMouseWheel(object? sender, MouseEventArgs e)
        {
            if (WindowsScaling == null || CurrentForm == null) return;
            var newX = e.X * WindowsScaling.Scaling;
            var newY = e.Y * WindowsScaling.Scaling;
            CurrentForm.OnMouseWheel(e, new RawVector2(newX, newY));
        }
        public void OnDragDrop(object? sender, DragEventArgs e)
        {
            if (WindowsScaling == null || CurrentForm == null) return;
            var newX = e.X * WindowsScaling.Scaling;
            var newY = e.Y * WindowsScaling.Scaling;
            CurrentForm.OnDragDrop(e, new RawVector2(newX, newY));
        }
        public void OnDragEnter(object? sender, DragEventArgs e)
        {
            if (WindowsScaling == null || CurrentForm == null) return;
            var newX = e.X * WindowsScaling.Scaling;
            var newY = e.Y * WindowsScaling.Scaling;
            CurrentForm.OnDragEnter(e, new RawVector2(newX, newY));
        }
        public void OnDragOver(object? sender, DragEventArgs e)
        {
            if (WindowsScaling == null || CurrentForm == null) return;
            var newX = e.X * WindowsScaling.Scaling;
            var newY = e.Y * WindowsScaling.Scaling;
            CurrentForm.OnDragOver(e, new RawVector2(newX, newY));
        }
        public void OnDragLeave(object? sender, EventArgs e)
        {
            if (CurrentForm == null) return;
            CurrentForm.OnDragLeave(e);
        }
    }
}