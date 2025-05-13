using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D;

public class ApplicationFormEvents(IApplicationForm applicationForm, IApplicationContext application)
{
    private Forms.Form CurrentForm => applicationForm.CurrentForm;

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
        var formPoint = new Point(e.X, e.Y);
        var ucPoint = applicationForm.PointToClient(formPoint);
        var newE = new DragEventArgs(e.Data, e.KeyState, ucPoint.X, ucPoint.Y, e.AllowedEffect, e.Effect);
        CurrentForm.OnDragDrop(newE);
    }
    public void OnDragEnter(object? sender, DragEventArgs e)
    {
        if (CurrentForm == null) return;
        var formPoint = new Point(e.X, e.Y);
        var ucPoint = applicationForm.PointToClient(formPoint);
        var newE = new DragEventArgs(e.Data, e.KeyState, ucPoint.X, ucPoint.Y, e.AllowedEffect, e.Effect);
        CurrentForm.OnDragEnter(newE);
    }
    public void OnDragOver(object? sender, DragEventArgs e)
    {
        if (CurrentForm == null) return;
        var formPoint = new Point(e.X, e.Y);
        var ucPoint = applicationForm.PointToClient(formPoint);
        var newE = new DragEventArgs(e.Data, e.KeyState, ucPoint.X, ucPoint.Y, e.AllowedEffect, e.Effect);
        CurrentForm.OnDragOver(newE);
    }
    public void OnDragLeave(object? sender, EventArgs e)
    {
        if (CurrentForm == null) return;
        CurrentForm.OnDragLeave(e);
    }

    internal void OnMouseEnter(object? sender, EventArgs e)
    {
        if (CurrentForm == null) return;
        CurrentForm.OnMouseEnter(e);
    }

    internal void OnMouseLeave(object? sender, EventArgs e)
    {
        if (CurrentForm == null) return;
        CurrentForm.OnMouseLeave(e);
    }
}