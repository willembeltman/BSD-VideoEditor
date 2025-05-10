using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class Control(IApplicationForm applicationForm, Form? parentForm, Control? parentControl)
{
    public IApplicationForm ApplicationForm { get; } = applicationForm;
    public Form? ParentForm { get; } = parentForm;
    public Control? ParentControl { get; } = parentControl;

    public bool Loaded { get; private set; }
    public bool Dirty { get; private set; }
    private int _Left = 0;
    private int _Top = 0;
    private int _Width = 480;
    private int _Height = 640;
    private Control[] Controls = [];
    private GraphicsLayer[] CanvasLayers = [];

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
    /// <summary>
    /// De base.OnDraw hoef je niet aan te roepen, dit wordt via invalidate geregeld
    /// </summary>
    public virtual void OnDraw()
    {
        // Hoeft niet door te gaan naar beneden, want dat doen we al met invalidate
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
    }
    public virtual void OnKeyUp(KeyEventArgs e)
    {
        foreach (var control in Controls)
        {
            control.OnKeyUp(e);
        }
    }
    public virtual void OnKeyDown(KeyEventArgs e)
    {
        foreach (var control in Controls)
        {
            control.OnKeyDown(e);
        }
    }
    public virtual void OnMouseClick(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                control.OnMouseClick(e);
            }
        }
    }
    public virtual void OnMouseDoubleClick(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                control.OnMouseDoubleClick(e);
            }
        }
    }
    public virtual void OnMouseUp(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                control.OnMouseUp(e);
            }
        }
    }
    public virtual void OnMouseDown(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                control.OnMouseDown(e);
            }
        }
    }
    public virtual void OnMouseMove(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                control.OnMouseMove(e);
            }
        }
    }
    public virtual void OnMouseWheel(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            if (control.Left < e.X && e.X < control.Right &&
                control.Top < e.Y && e.Y < control.Bottom)
            {
                control.OnMouseWheel(e);
            }
        }
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
    }
    public virtual void OnDragLeave(EventArgs e)
    {
        foreach (var control in Controls)
        {
            control.OnDragLeave(e);
        }
    }
    public virtual void OnUpdate()
    {
        foreach (var control in Controls)
        {
            control.OnUpdate();
        }
    }

    public void AddControl(Control control)
    {
        var newArray = new Control[Controls.Length + 1];
        Array.Copy(Controls, newArray, Controls.Length);
        newArray[^1] = control;
        Controls = newArray;

        Invalidate();
    }
    public void RemoveControl(Control control)
    {
        var deleted = 0;
        for (int i = 0; i < Controls.Length; i++)
        {
            if (Controls[i] == control)
            {
                deleted++;
            }
        }
        var newArray = new Control[Controls.Length - deleted];
        var newIndex = 0;
        for (int i = 0; i < Controls.Length; i++)
        {
            if (Controls[i] != control)
            {
                newArray[newIndex] = Controls[i];
                newIndex++;
            }
        }
        Controls = newArray;

        Invalidate();
    }

    public void AddCanvasLayer(GraphicsLayer layer)
    {
        var newArray = new GraphicsLayer[CanvasLayers.Length + 1];
        Array.Copy(CanvasLayers, newArray, CanvasLayers.Length);
        newArray[^1] = layer;
        CanvasLayers = newArray;
    }
    public void RemoveCanvasLayer(GraphicsLayer layer)
    {
        var deleted = 0;
        for (int i = 0; i < CanvasLayers.Length; i++)
        {
            if (CanvasLayers[i] == layer)
            {
                deleted++;
            }
        }
        var newArray = new GraphicsLayer[CanvasLayers.Length - deleted];
        var newIndex = 0;
        for (int i = 0; i < CanvasLayers.Length; i++)
        {
            if (CanvasLayers[i] != layer)
            {
                newArray[newIndex] = CanvasLayers[i];
                newIndex++;
            }
        }
        CanvasLayers = newArray;
    }
    public GraphicsLayer CreateCanvasLayer()
    {
        var layer = new GraphicsLayer(ApplicationForm);
        AddCanvasLayer(layer);
        return layer;
    }

    public IEnumerable<GraphicsLayer> GetCanvasLayers()
    {
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
            foreach (var layer in control.GetCanvasLayers())
            {
                yield return layer;
            }
        }
    }
}
