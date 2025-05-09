using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D.Forms;

public abstract class ControlD3D(FormD3D? parentForm)
{
    private FormD3D? _ParentForm = parentForm;
    private int _Left = 0;
    private int _Top = 0;
    private int _Width = 0;
    private int _Height = 0;

    private CanvasLayer[] CanvasLayers = [];
    private bool Validated;

    public virtual FormD3D ParentForm => _ParentForm!;
    public virtual void OnResize()
    {
        Invalidate();
    }
    public abstract void Draw();
    public void Invalidate()
    {
        Validated = false;
    }

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

    private RawColor4 _BackgroundColor;
    public RawColor4 BackgroundColor
    {
        get => _BackgroundColor;
        set
        {
            _BackgroundColor = value;
            Invalidate();
        }
    }
    private RawColor4 _ForegroundColor;
    public RawColor4 ForegroundColor
    {
        get => _ForegroundColor;
        set
        {
            _ForegroundColor = value;
            Invalidate();
        }
    }

    public ControlD3D[] Controls { get; set; } = [];
    public void AddControl(ControlD3D control)
    {
        var newArray = new ControlD3D[Controls.Length + 1];
        Array.Copy(Controls, newArray, Controls.Length);
        newArray[newArray.Length - 1] = control;
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
        newArray[newArray.Length - 1] = layer;
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
        var layer = new CanvasLayer(ParentForm.Device, ParentForm.Characters, ParentForm.Width, ParentForm.Height);
        AddCanvasLayer(layer);
        return layer;
    }

    public IEnumerable<CanvasLayer> GetLayers()
    {
        if (!Validated)
        {
            Draw();
            Validated = true;
        }

        foreach (var layer in CanvasLayers)
        {
            yield return layer;
        }
        foreach (var control in Controls)
        {
            foreach (var layer in control.GetLayers())
            {
                yield return layer;
            }
        }
    }
}
