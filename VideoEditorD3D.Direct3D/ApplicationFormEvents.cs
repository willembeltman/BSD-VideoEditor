using VideoEditorD3D.Direct3D.Events;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D;

public class ApplicationFormEvents(IApplicationForm applicationForm)
{
    public void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        foreach (var form in applicationForm.Forms) form.OnKeyPress(e);
    }
    public void OnKeyUp(object? sender, KeyEventArgs e)
    {
        foreach (var form in applicationForm.Forms) form.OnKeyUp(e);
    }
    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        foreach (var form in applicationForm.Forms) form.OnKeyDown(e);
    }

    public void OnMouseClick(object? sender, MouseEventArgs e)
    {
        foreach (var form in applicationForm.Forms) form.OnMouseClick(new MouseEvent(form, e));
    }
    public void OnMouseDoubleClick(object? sender, MouseEventArgs e)
    {
        foreach (var form in applicationForm.Forms) form.OnMouseDoubleClick(new MouseEvent(form, e));
    }
    public void OnMouseUp(object? sender, MouseEventArgs e)
    {
        foreach (var form in applicationForm.Forms) form.OnMouseUp(new MouseEvent(form, e));
    }
    public void OnMouseDown(object? sender, MouseEventArgs e)
    {
        foreach (var form in applicationForm.Forms) form.OnMouseDown(new MouseEvent(form, e));
    }
    public void OnMouseMove(object? sender, MouseEventArgs e)
    {
        foreach (var form in applicationForm.Forms) form.OnMouseMove(new MouseEvent(form, e));
    }
    public void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        foreach (var form in applicationForm.Forms) form.OnMouseWheel(new MouseEvent(form, e));
    }
    public void OnMouseEnter(object? sender, EventArgs e)
    {
        foreach (var popup in applicationForm.Forms) popup.OnMouseEnter(e);
    }
    public void OnMouseLeave(object? sender, EventArgs e)
    {
        foreach (var popup in applicationForm.Forms) popup.OnMouseLeave(e);
    }

    public void OnDragEnter(object? sender, DragEventArgs e)
    {
        var desktopPoint = new Point(e.X, e.Y);
        var formPoint = applicationForm.PointToClient(desktopPoint);
        var e2 = new DragEvent(e, formPoint.X, formPoint.Y);
        foreach (var popup in applicationForm.Forms)
        {
            //if (popup.Left <= e.X && e.X <= popup.Right &&
            //    popup.Top <= e.Y && e.Y <= popup.Bottom)
            //{
            var newE = new DragEvent(popup, e2);
            popup.OnDragEnter(newE);
            //}
        }
    }
    public void OnDragOver(object? sender, DragEventArgs e)
    {
        var desktopPoint = new Point(e.X, e.Y);
        var formPoint = applicationForm.PointToClient(desktopPoint);
        var e2 = new DragEvent(e, formPoint.X, formPoint.Y);
        foreach (var popup in applicationForm.Forms)
        {
            //if (popup.Left <= e.X && e.X <= popup.Right &&
            //    popup.Top <= e.Y && e.Y <= popup.Bottom)
            //{
            var newE = new DragEvent(popup, e2);
            popup.OnDragOver(newE);
            //}
        }
    }
    public void OnDragDrop(object? sender, DragEventArgs e)
    {
        var desktopPoint = new Point(e.X, e.Y);
        var formPoint = applicationForm.PointToClient(desktopPoint);
        var e2 = new DragEvent(e, formPoint.X, formPoint.Y);
        foreach (var popup in applicationForm.Forms)
        {
            //if (popup.Left <= e.X && e.X <= popup.Right &&
            //    popup.Top <= e.Y && e.Y <= popup.Bottom)
            //{
            var newE = new DragEvent(popup, e2);
            popup.OnDragDrop(newE);
            //}
        }
    }
    public void OnDragLeave(object? sender, EventArgs e)
    {
        foreach (var popup in applicationForm.Forms) popup.OnDragLeave(e);
    }
}