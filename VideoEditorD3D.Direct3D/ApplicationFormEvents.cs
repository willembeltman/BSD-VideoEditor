using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D;

public class ApplicationFormEvents(IApplicationForm applicationForm)
{
    Controls.Control Form => applicationForm.CurrentForm;

    public void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups) popup.OnKeyPress(e);
        Form.OnKeyPress(e);
    }
    public void OnKeyUp(object? sender, KeyEventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups) popup.OnKeyUp(e);
        Form.OnKeyUp(e);
    }
    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups) popup.OnKeyDown(e);
        Form.OnKeyDown(e);
    }
    public void OnMouseClick(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups)
        {
            if (popup.Left <= e.X && e.X <= popup.Right &&
                popup.Top <= e.Y && e.Y <= popup.Bottom)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - popup.Left, e.Y - popup.Top, e.Delta);
                popup.OnMouseClick(newE);
            }
        }
        Form.OnMouseClick(e);
    }
    public void OnMouseDoubleClick(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups)
        {
            var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - popup.Left, e.Y - popup.Top, e.Delta);
            popup.OnMouseDoubleClick(newE);
        }
        Form.OnMouseDoubleClick(e);
    }
    public void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups)
        {
            if (popup.Left <= e.X && e.X <= popup.Right &&
                popup.Top <= e.Y && e.Y <= popup.Bottom)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - popup.Left, e.Y - popup.Top, e.Delta);
                popup.OnMouseUp(newE);
            }
        }
        Form.OnMouseUp(e);
    }
    public void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups)
        {
            if (popup.Left <= e.X && e.X <= popup.Right &&
                popup.Top <= e.Y && e.Y <= popup.Bottom)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - popup.Left, e.Y - popup.Top, e.Delta);
                popup.OnMouseDown(newE);
            }
        }
        Form.OnMouseDown(e);
    }
    public void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups)
        {
            if (popup.Left <= e.X && e.X <= popup.Right &&
                popup.Top <= e.Y && e.Y <= popup.Bottom)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - popup.Left, e.Y - popup.Top, e.Delta);
                popup.OnMouseMove(newE);
            }
        }
        Form.OnMouseMove(e);
    }
    public void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups)
        {
            if (popup.Left <= e.X && e.X <= popup.Right &&
                popup.Top <= e.Y && e.Y <= popup.Bottom)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - popup.Left, e.Y - popup.Top, e.Delta);
                popup.OnMouseWheel(newE);
            }
        }
        Form.OnMouseWheel(e);
    }
    public void OnDragDrop(object? sender, DragEventArgs e)
    {
        if (Form == null) return;
        var desktopPoint = new Point(e.X, e.Y);
        var formPoint = applicationForm.PointToClient(desktopPoint);
        var e2 = new DragEvent(e, formPoint.X, formPoint.Y);
        //e = new DragEventArgs(e.Data, e.KeyState, formPoint.X, formPoint.Y, e.AllowedEffect, e.Effect);
        foreach (var popup in applicationForm.Popups)
        {
            if (popup.Left <= e.X && e.X <= popup.Right &&
                popup.Top <= e.Y && e.Y <= popup.Bottom)
            {
                var newE = new DragEvent(popup, e);
                //var newE = new DragEventArgs(e.Data, e.KeyState, e.X - popup.Left, e.Y - popup.Top, e.AllowedEffect, e.Effect);
                popup.OnDragDrop(newE);
            }
        }
        Form.OnDragDrop(e2);
    }
    public void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (Form == null) return;
        var desktopPoint = new Point(e.X, e.Y);
        var formPoint = applicationForm.PointToClient(desktopPoint);
        var e2 = new DragEvent(e, formPoint.X, formPoint.Y);
        //e = new DragEventArgs(e.Data, e.KeyState, formPoint.X, formPoint.Y, e.AllowedEffect, e.Effect);
        foreach (var popup in applicationForm.Popups)
        {
            if (popup.Left <= e.X && e.X <= popup.Right &&
                popup.Top <= e.Y && e.Y <= popup.Bottom)
            {
                var newE = new DragEvent(popup, e);
                //var newE = new DragEventArgs(e.Data, e.KeyState, e.X - popup.Left, e.Y - popup.Top, e.AllowedEffect, e.Effect);
                popup.OnDragEnter(newE);
            }
        }
        Form.OnDragEnter(e2);
    }
    public void OnDragOver(object? sender, DragEventArgs e)
    {
        if (Form == null) return;
        var desktopPoint = new Point(e.X, e.Y);
        var formPoint = applicationForm.PointToClient(desktopPoint);
        var e2 = new DragEvent(e, formPoint.X, formPoint.Y);
        //e = new DragEventArgs(e.Data, e.KeyState, formPoint.X, formPoint.Y, e.AllowedEffect, e.Effect);
        foreach (var popup in applicationForm.Popups)
        {
            if (popup.Left <= e.X && e.X <= popup.Right &&
                popup.Top <= e.Y && e.Y <= popup.Bottom)
            {
                var newE = new DragEvent(popup, e);
                //var newE = new DragEventArgs(e.Data, e.KeyState, e.X - popup.Left, e.Y - popup.Top, e.AllowedEffect, e.Effect);
                popup.OnDragOver(newE);
            }
        }
        Form.OnDragOver(e2);
    }
    public void OnDragLeave(object? sender, EventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups) popup.OnDragLeave(e);
        Form.OnDragLeave(e);
    }

    internal void OnMouseEnter(object? sender, EventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups) popup.OnMouseEnter(e);
        Form.OnMouseEnter(e);
    }

    internal void OnMouseLeave(object? sender, EventArgs e)
    {
        if (Form == null) return;
        foreach (var popup in applicationForm.Popups) popup.OnMouseLeave(e);
        Form.OnMouseLeave(e);
    }
}