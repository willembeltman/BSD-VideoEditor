using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Forms;

public class ControlD3D(IApplication application, FormD3D? parentForm, ControlD3D? parentControl)
{
    public virtual IApplication Application { get; } = application;
    public FormD3D? ParentForm { get; } = parentForm;
    public ControlD3D? ParentControl { get; } = parentControl;

    private bool Dirty;
    private int _Left = 0;
    private int _Top = 0;
    private int _Width = 480;
    private int _Height = 640;
    private RawColor4 _BackgroundColor;
    private ControlD3D[] Controls = [];
    private CanvasLayer[] CanvasLayers = [];

    public int Left
    {
        get => _Left;
        set
        {
            if (_Left != value)
            {
                _Left = value;
                OnResize();
            }
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
    public RawColor4 BackgroundColor
    {
        get => _BackgroundColor;
        set
        {
            _BackgroundColor = value;
            Invalidate();
        }
    }

    public virtual void OnDraw()
    {

    }
    public void Invalidate()
    {
        foreach (var control in Controls)
        {
            control.Invalidate();
        }
        Dirty = true;
    }
    public virtual void OnResize()
    {
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
    public virtual void OnMouseClick(MouseEventArgs e, RawVector2 position)
    {
        foreach (var control in Controls)
        {
            if (control.Left < position.X && position.X < control.Right &&
                control.Top < position.Y && position.Y < control.Bottom)
            {
                control.OnMouseClick(e, position);
            }
        }
    }
    public virtual void OnMouseDoubleClick(MouseEventArgs e, RawVector2 position)
    {
        foreach (var control in Controls)
        {
            if (control.Left < position.X && position.X < control.Right &&
                control.Top < position.Y && position.Y < control.Bottom)
            {
                control.OnMouseDoubleClick(e, position);
            }
        }
    }
    public virtual void OnMouseUp(MouseEventArgs e, RawVector2 position)
    {
        foreach (var control in Controls)
        {
            if (control.Left < position.X && position.X < control.Right &&
                control.Top < position.Y && position.Y < control.Bottom)
            {
                control.OnMouseUp(e, position);
            }
        }
    }
    public virtual void OnMouseDown(MouseEventArgs e, RawVector2 position)
    {
        foreach (var control in Controls)
        {
            if (control.Left < position.X && position.X < control.Right &&
                control.Top < position.Y && position.Y < control.Bottom)
            {
                control.OnMouseDown(e, position);
            }
        }
    }
    public virtual void OnMouseMove(MouseEventArgs e, RawVector2 position)
    {
        foreach (var control in Controls)
        {
            if (control.Left < position.X && position.X < control.Right &&
                control.Top < position.Y && position.Y < control.Bottom)
            {
                control.OnMouseMove(e, position);
            }
        }
    }
    public virtual void OnMouseWheel(MouseEventArgs e, RawVector2 position)
    {
        foreach (var control in Controls)
        {
            if (control.Left < position.X && position.X < control.Right &&
                control.Top < position.Y && position.Y < control.Bottom)
            {
                control.OnMouseWheel(e, position);
            }
        }
    }
    public virtual void OnUpdate()
    {
        foreach (var control in Controls)
        {
            control.OnUpdate();
        }
    }

    public void AddControl(ControlD3D control)
    {
        var newArray = new ControlD3D[Controls.Length + 1];
        Array.Copy(Controls, newArray, Controls.Length);
        newArray[^1] = control;
        Controls = newArray;

        Invalidate();
    }
    public void RemoveControl(ControlD3D control)
    {
        var deleted = 0;
        for (int i = 0; i < Controls.Length; i++)
        {
            if (Controls[i] == control)
            {
                deleted++;
            }
        }
        var newArray = new ControlD3D[Controls.Length - deleted];
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

    public void AddCanvasLayer(CanvasLayer layer)
    {
        var newArray = new CanvasLayer[CanvasLayers.Length + 1];
        Array.Copy(CanvasLayers, newArray, CanvasLayers.Length);
        newArray[^1] = layer;
        CanvasLayers = newArray;
    }
    public void RemoveCanvasLayer(CanvasLayer layer)
    {
        var deleted = 0;
        for (int i = 0; i < CanvasLayers.Length; i++)
        {
            if (CanvasLayers[i] == layer)
            {
                deleted++;
            }
        }
        var newArray = new CanvasLayer[CanvasLayers.Length - deleted];
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
    public CanvasLayer CreateCanvasLayer()
    {
        var layer = new CanvasLayer(Application);
        AddCanvasLayer(layer);
        return layer;
    }

    public IEnumerable<CanvasLayer> GetCanvasLayers()
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
