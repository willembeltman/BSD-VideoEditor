using SharpDX.Mathematics.Interop;

namespace VideoEditorD3D.Direct3D.Forms;

public class ControlD3D(FormD3D? parentForm)
{
    private readonly FormD3D? _ParentForm = parentForm;
    public virtual FormD3D ParentForm => _ParentForm!;
    public IApplication Application => ParentForm.Application;

    public virtual void OnResize()
    {
        Invalidate();
    }

    public virtual void Draw()
    {

    }

    private bool Validated;
    public void Invalidate()
    {
        foreach (var control in Controls)
        {
            control.Invalidate();
        }
        Validated = false;
    }

    private int _Left = 0;
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
     
    private int _Top = 0;
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

    private int _Width = 480;
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

    private int _Height = 640;
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

    private ControlD3D[] Controls = [];
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

    private CanvasLayer[] CanvasLayers = [];
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
            foreach (var layer in control.GetCanvasLayers())
            {
                yield return layer;
            }
        }
    }
}
