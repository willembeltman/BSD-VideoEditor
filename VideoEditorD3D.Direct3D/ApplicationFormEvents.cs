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
        var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - Form.Left, e.Y - Form.Top, e.Delta);
        foreach (var popup in applicationForm.Popups) if (popup.OnMouseClick(newE)) return;
        Form.OnMouseClick(newE);
    }
    public void OnMouseDoubleClick(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - Form.Left, e.Y - Form.Top, e.Delta);
        foreach (var popup in applicationForm.Popups) popup.OnMouseDoubleClick(newE);
        Form.OnMouseDoubleClick(newE);
    }
    public void OnMouseUp(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - Form.Left, e.Y - Form.Top, e.Delta);
        foreach (var popup in applicationForm.Popups) popup.OnMouseUp(newE);
        Form.OnMouseUp(newE);
    }
    public void OnMouseDown(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - Form.Left, e.Y - Form.Top, e.Delta);
        foreach (var popup in applicationForm.Popups) popup.OnMouseDown(newE);
        Form.OnMouseDown(newE);
    }
    public void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - Form.Left, e.Y - Form.Top, e.Delta);
        foreach (var popup in applicationForm.Popups) popup.OnMouseMove(newE);
        Form.OnMouseMove(newE);
    }
    public void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        if (Form == null) return;
        var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - Form.Left, e.Y - Form.Top, e.Delta);
        foreach (var popup in applicationForm.Popups) popup.OnMouseWheel(newE);
        Form.OnMouseWheel(newE);
    }
    public void OnDragDrop(object? sender, DragEventArgs e)
    {
        if (Form == null) return;
        var formPoint = new Point(e.X, e.Y);
        var ucPoint = applicationForm.PointToClient(formPoint);
        var newE = new DragEventArgs(e.Data, e.KeyState, ucPoint.X - Form.Left, ucPoint.Y - Form.Top, e.AllowedEffect, e.Effect);
        foreach (var popup in applicationForm.Popups) popup.OnDragDrop(newE);
        Form.OnDragDrop(newE);
    }
    public void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (Form == null) return;
        var formPoint = new Point(e.X, e.Y);
        var ucPoint = applicationForm.PointToClient(formPoint);
        var newE = new DragEventArgs(e.Data, e.KeyState, ucPoint.X - Form.Left, ucPoint.Y - Form.Top, e.AllowedEffect, e.Effect);
        foreach (var popup in applicationForm.Popups) popup.OnDragEnter(newE);
        Form.OnDragEnter(newE);
    }
    public void OnDragOver(object? sender, DragEventArgs e)
    {
        if (Form == null) return;
        var formPoint = new Point(e.X, e.Y);
        var ucPoint = applicationForm.PointToClient(formPoint);
        var newE = new DragEventArgs(e.Data, e.KeyState, ucPoint.X - Form.Left, ucPoint.Y - Form.Top, e.AllowedEffect, e.Effect);
        foreach (var popup in applicationForm.Popups) popup.OnDragOver(newE);
        Form.OnDragOver(newE);
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