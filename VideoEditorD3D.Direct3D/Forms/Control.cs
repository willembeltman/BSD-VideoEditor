using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class Control : IDisposable
{
    public IApplicationForm ApplicationForm { get; }
    public Form? ParentForm { get; }
    public Control? ParentControl { get; }
    public ControlCollection Controls { get; }
    public GraphicsLayerCollection CanvasLayers { get; }

    public bool Loaded { get; private set; }
    public bool Dirty { get; private set; }

    private int _Left = 0;
    private int _Top = 0;
    private int _Width = 480;
    private int _Height = 640;

    public Control(IApplicationForm applicationForm, Form? parentForm, Control? parentControl)
    {
        ApplicationForm = applicationForm;
        ParentForm = parentForm;
        ParentControl = parentControl;
        Controls = new ControlCollection(this);
        CanvasLayers = new GraphicsLayerCollection(this);
    }

    public bool IsMouseEntered { get; private set; } = false;

    public event EventHandler<KeyPressEventArgs>? KeyPress;
    public event EventHandler<KeyEventArgs>? KeyUp;
    public event EventHandler<KeyEventArgs>? KeyDown;
    public event EventHandler<MouseEventArgs>? MouseClick;
    public event EventHandler<MouseEventArgs>? MouseDoubleClick;
    public event EventHandler<MouseEventArgs>? MouseUp;
    public event EventHandler<MouseEventArgs>? MouseDown;
    public event EventHandler<MouseEventArgs>? MouseMove;
    public event EventHandler<MouseEventArgs>? MouseWheel;
    public event EventHandler<EventArgs>? MouseEnter;
    public event EventHandler<EventArgs>? MouseLeave;
    public event EventHandler<DragEventArgs>? DragEnter;
    public event EventHandler<DragEventArgs>? DragOver;
    public event EventHandler<EventArgs>? DragLeave;
    public event EventHandler<DragEventArgs>? DragDrop;

    public virtual bool Visible { get; set; } = true;
    public int Left
    {
        get => _Left;
        set
        {
            if (_Left == value) return;
            _Left = value;
            OnResize();
        }
    }
    public int Top
    {
        get => _Top;
        set
        {
            if (_Top != value)
            {
                _Top = value;
                OnResize();
            }
        }
    }
    public int Width
    {
        get => _Width;
        set
        {
            if (_Width != value)
            {
                _Width = value;
                OnResize();
            }
        }
    }
    public int Height
    {
        get => _Height;
        set
        {
            if (_Height != value)
            {
                _Height = value;
                OnResize();
            }
        }
    }
    public int Right => Left + Width;
    public int Bottom => Top + Height;

    public int AbsoluteLeft
    {
        get
        {
            var control = ParentControl;
            var left = Left;
            while (control != null)
            {
                left += control.Left;
                control = control.ParentControl;
            }
            return left;
        } 
    }
    public int AbsoluteTop
    {
        get
        {
            var control = ParentControl;
            var top = Top;
            while (control != null)
            {
                top += control.Top;
                control = control.ParentControl;
            }
            return top;
        }
    }
    public int AbsoluteRight => AbsoluteLeft + Width;
    public int AbsoluteBottom => AbsoluteTop + Height;

    public void Invalidate()
    {
        foreach (var control in Controls)
        {
            control.Invalidate();
        }
        Dirty = true;
    }
    public virtual void OnLoad()
    {
        foreach (var control in Controls)
        {
            control.OnLoad();
        }
        Loaded = true;
    }
    public virtual void OnDraw()
    {
    }
    public virtual void OnResize()
    {
        // Hoeft niet door te gaan naar beneden, want dat doet als het goed is het Form of Control al
        Invalidate();
    }
    public virtual void OnKeyPress(KeyPressEventArgs e)
    {
        foreach (var control in Controls)
        {
            control.OnKeyPress(e);
        }
        KeyPress?.Invoke(this, e);
    }
    public virtual void OnKeyUp(KeyEventArgs e)
    {
        foreach (var control in Controls)
        {
            control.OnKeyUp(e);
        }
        KeyUp?.Invoke(this, e);
    }
    public virtual void OnKeyDown(KeyEventArgs e)
    {
        foreach (var control in Controls)
        {
            control.OnKeyDown(e);
        }
        KeyDown?.Invoke(this, e);
    }

    public virtual void OnMouseClick(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);
                control.OnMouseClick(newE);
            }
        }

        MouseClick?.Invoke(this, e);
    }
    public virtual void OnMouseDoubleClick(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);
                control.OnMouseDoubleClick(newE);
            }
        }
        MouseDoubleClick?.Invoke(this, e);
    }
    public virtual void OnMouseUp(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);
                control.OnMouseUp(newE);
            }
        }
        MouseUp?.Invoke(this, e);
    }
    public virtual void OnMouseDown(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);
                control.OnMouseDown(newE);
            }
        }
        MouseDown?.Invoke(this, e);
    }
    public virtual void OnMouseMove(MouseEventArgs e)
    {
        IsMouseEntered = true;
        foreach (var control in Controls)
        {
            var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);

            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                if (!control.IsMouseEntered)
                {
                    control.OnMouseEnter(newE);
                }
                control.OnMouseMove(newE);

            }
            else
            {
                if (control.IsMouseEntered)
                {
                    control.OnMouseLeave(newE);
                }
            }
        }
        MouseMove?.Invoke(this, e);
    }
    public virtual void OnMouseWheel(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);
                control.OnMouseWheel(newE);
            }
        }
        MouseWheel?.Invoke(this, e);
    }
    public virtual void OnMouseEnter(EventArgs e)
    {
        IsMouseEntered = true;
        MouseEnter?.Invoke(this, e);
    }
    public virtual void OnMouseLeave(EventArgs e)
    {
        IsMouseEntered = false;
        foreach (var control in Controls)
        {
            if (control.IsMouseEntered)
            {
                control.OnMouseLeave(e);
            }
        }
        MouseLeave?.Invoke(this, e);
    }

    public virtual void OnDragEnter(DragEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                control.OnDragEnter(e);
            }
        }
        DragEnter?.Invoke(this, e);
    }
    public virtual void OnDragOver(DragEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                control.OnDragOver(e);
            }
        }
        DragOver?.Invoke(this, e);
    }
    public virtual void OnDragLeave(EventArgs e)
    {
        foreach (var control in Controls)
        {
            control.OnDragLeave(e);
        }
        DragLeave?.Invoke(this, e);
    }
    public virtual void OnDragDrop(DragEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                control.OnDragDrop(e);
            }
        }
        DragDrop?.Invoke(this, e);
    }

    public virtual void OnUpdate()
    {
        foreach (var control in Controls)
        {
            control.OnUpdate();
        }
    }
    public virtual void OnDispose()
    {
    }


    public IEnumerable<GraphicsLayer> GetAllCanvasLayers()
    {
        if (!Visible) yield break;

        if (Dirty)
        {
            OnDraw();
            Dirty = false;
        }

        foreach (var layer in CanvasLayers)
        {
            yield return layer;
        }
        foreach (var control in Controls)
        {
            foreach (var layer in control.GetAllCanvasLayers())
            {
                yield return layer;
            }
        }
    }

    public void Dispose()
    {
        OnDispose();
        foreach (var layer in CanvasLayers)
        {
            layer.Dispose();
        }
        foreach (var control in Controls)
        {
            control.Dispose();
        }
    }
}
