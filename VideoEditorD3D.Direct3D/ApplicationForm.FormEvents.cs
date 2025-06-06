using VideoEditorD3D.Direct3D.Events;

namespace VideoEditorD3D.Direct3D;

public partial class ApplicationForm
{
    public void OnKeyPress(object? sender, KeyPressEventArgs e)
    {
        foreach (var form in _Forms!) form.OnKeyPress(e);
    }
    public void OnKeyUp(object? sender, KeyEventArgs e)
    {
        foreach (var form in _Forms!) form.OnKeyUp(e);
    }
    public void OnKeyDown(object? sender, KeyEventArgs e)
    {
        foreach (var form in _Forms!) form.OnKeyDown(e);
    }

    public void OnMouseClick(object? sender, MouseEventArgs e)
    {
        foreach (var form in _Forms!) form.OnMouseClick(new MouseEvent(form, e));
    }
    public void OnMouseDoubleClick(object? sender, MouseEventArgs e)
    {
        foreach (var form in _Forms!) form.OnMouseDoubleClick(new MouseEvent(form, e));
    }
    public void OnMouseUp(object? sender, MouseEventArgs e)
    {
        foreach (var form in _Forms!) form.OnMouseUp(new MouseEvent(form, e));
    }
    public void OnMouseDown(object? sender, MouseEventArgs e)
    {
        foreach (var form in _Forms!) form.OnMouseDown(new MouseEvent(form, e));
    }
    public void OnMouseMove(object? sender, MouseEventArgs e)
    {
        foreach (var form in _Forms!) form.OnMouseMove(new MouseEvent(form, e));
    }
    public void OnMouseWheel(object? sender, MouseEventArgs e)
    {
        foreach (var form in _Forms!) form.OnMouseWheel(new MouseEvent(form, e));
    }
    public void OnMouseEnter(object? sender, EventArgs e)
    {
        foreach (var form in _Forms!) form.OnMouseEnter(e);
    }
    public void OnMouseLeave(object? sender, EventArgs e)
    {
        foreach (var form in _Forms!) form.OnMouseLeave(e);
    }

    public void OnDragEnter(object? sender, DragEventArgs e)
    {
        var desktopPoint = new System.Drawing.Point(e.X, e.Y);
        var formPoint = PointToClient(desktopPoint);
        var e2 = new DragEvent(e, formPoint.X, formPoint.Y);
        foreach (var form in _Forms!)
        {
            //if (popup.Left <= e.X && e.X <= popup.Right &&
            //    popup.Top <= e.Y && e.Y <= popup.Bottom)
            //{
            var newE = new DragEvent(form, e2);
            form.OnDragEnter(newE);
            //}
        }
    }
    public void OnDragOver(object? sender, DragEventArgs e)
    {
        var desktopPoint = new System.Drawing.Point(e.X, e.Y);
        var formPoint = PointToClient(desktopPoint);
        var e2 = new DragEvent(e, formPoint.X, formPoint.Y);
        foreach (var form in _Forms!)
        {
            //if (popup.Left <= e.X && e.X <= popup.Right &&
            //    popup.Top <= e.Y && e.Y <= popup.Bottom)
            //{
            var newE = new DragEvent(form, e2);
            form.OnDragOver(newE);
            //}
        }
    }
    public void OnDragDrop(object? sender, DragEventArgs e)
    {
        var desktopPoint = new System.Drawing.Point(e.X, e.Y);
        var formPoint = PointToClient(desktopPoint);
        var e2 = new DragEvent(e, formPoint.X, formPoint.Y);
        foreach (var form in _Forms!)
        {
            //if (popup.Left <= e.X && e.X <= popup.Right &&
            //    popup.Top <= e.Y && e.Y <= popup.Bottom)
            //{
            var newE = new DragEvent(form, e2);
            form.OnDragDrop(newE);
            //}
        }
    }
    public void OnDragLeave(object? sender, EventArgs e)
    {
        foreach (var form in _Forms!) form.OnDragLeave(e);
    }
}
