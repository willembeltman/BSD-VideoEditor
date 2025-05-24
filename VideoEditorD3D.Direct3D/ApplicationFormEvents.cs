using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D;

public class ApplicationFormEvents(IApplicationForm applicationForm)
{
    public void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        foreach (var popup in applicationForm.Forms) popup.OnKeyPress(e);
    }
    public void OnKeyUp(object? sender, KeyEventArgs e)
    {
        foreach (var popup in applicationForm.Forms) popup.OnKeyUp(e);
    }
    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        foreach (var popup in applicationForm.Forms) popup.OnKeyDown(e);
    }
    public void OnMouseClick(object? sender, MouseEventArgs e)
    {
        foreach (var popup in applicationForm.Forms)
        {
            if (popup.Left <= e.X && e.X <= popup.Right &&
                popup.Top <= e.Y && e.Y <= popup.Bottom)
            {
                var newE = new MouseEvent(popup, e);
                popup.OnMouseClick(newE);
            }
        }
    }
    public void OnMouseDoubleClick(object? sender, MouseEventArgs e)
    {
        foreach (var popup in applicationForm.Forms)
        {
            var newE = new MouseEvent(popup, e);
            popup.OnMouseDoubleClick(newE);
        }
    }
    public void OnMouseUp(object? sender, MouseEventArgs e)
    {
        foreach (var popup in applicationForm.Forms)
        {
            //if (popup.Left <= e.X && e.X <= popup.Right &&
            //    popup.Top <= e.Y && e.Y <= popup.Bottom)
            //{
                var newE = new MouseEvent(popup, e);
                popup.OnMouseUp(newE);
            //}
        }
    }
    public void OnMouseDown(object? sender, MouseEventArgs e)
    {
        foreach (var popup in applicationForm.Forms)
        {
            //if (popup.Left <= e.X && e.X <= popup.Right &&
            //    popup.Top <= e.Y && e.Y <= popup.Bottom)
            //{
                var newE = new MouseEvent(popup, e);
                popup.OnMouseDown(newE);
            //}
        }
    }
    public void OnMouseMove(object? sender, MouseEventArgs e)
    {
        foreach (var popup in applicationForm.Forms)
        {
            //if (popup.Left <= e.X && e.X <= popup.Right &&
            //    popup.Top <= e.Y && e.Y <= popup.Bottom)
            //{
                var newE = new MouseEvent(popup, e);
                popup.OnMouseMove(newE);
            //}
        }
    }
    public void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        foreach (var popup in applicationForm.Forms)
        {
            //if (popup.Left <= e.X && e.X <= popup.Right &&
            //    popup.Top <= e.Y && e.Y <= popup.Bottom)
            //{
                var newE = new MouseEvent(popup, e);
                popup.OnMouseWheel(newE);
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
    public void OnDragLeave(object? sender, EventArgs e)
    {
        foreach (var popup in applicationForm.Forms) popup.OnDragLeave(e);
    }

    public void OnMouseEnter(object? sender, EventArgs e)
    {
        foreach (var popup in applicationForm.Forms) popup.OnMouseEnter(e);
    }

    public void OnMouseLeave(object? sender, EventArgs e)
    {
        foreach (var popup in applicationForm.Forms) popup.OnMouseLeave(e);
    }
}