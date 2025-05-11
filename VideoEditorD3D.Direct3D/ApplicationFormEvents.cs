using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D;

public class ApplicationFormEvents
{
    private IApplicationForm applicationForm;
    private IApplicationContext application;
    private Forms.Form CurrentForm => applicationForm.CurrentForm;

    public ApplicationFormEvents(IApplicationForm applicationForm, IApplicationContext application)
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