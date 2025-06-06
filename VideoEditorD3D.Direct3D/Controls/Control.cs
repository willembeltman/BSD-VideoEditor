using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Events;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls;

public class Control : IDisposable
{

    private int _Left = 0;
    private int _Top = 0;
    private int _Width = 80;
    private int _Height = 36;
    private bool HasToResize = false;

    // Disable because properties are injected when control is added to parent control,
    // this is solved by the: Constructor => Load => Update => Draw lifecycle.
    // (ok, maybe nulleble would have been better but then I would have written ! everywhere) 
#nullable disable 
    public IApplicationForm ApplicationForm { get; internal set; }
    public Form ParentForm { get; internal set; }
    public Control ParentControl { get; internal set; }
#nullable enable

    public ControlCollection Controls { get; protected set; }
    public GraphicsLayerCollection GraphicsLayers { get; protected set; }

    public bool HasFocus { get; set; }
    public bool Loaded { get; private set; } = false;
    public bool HasToDraw { get; set; } = true;
    public bool MouseOver { get; set; } = false;
    public bool Visible { get; set; } = true;
    public bool DragEntered { get; private set; }

    public Control()
    {
        Controls = new ControlCollection(this);
        GraphicsLayers = new GraphicsLayerCollection(this);
    }

    public event EventHandler<EventArgs>? Load;
    public event EventHandler<EventArgs>? Draw;
    public event EventHandler<EventArgs>? Update;
    public event EventHandler<KeyPressEventArgs>? KeyPress;
    public event EventHandler<KeyEventArgs>? KeyUp;
    public event EventHandler<KeyEventArgs>? KeyDown;
    public event EventHandler<MouseEvent>? Click;
    public event EventHandler<MouseEvent>? MouseDoubleClick;
    public event EventHandler<MouseEvent>? MouseUp;
    public event EventHandler<MouseEvent>? MouseDown;
    public event EventHandler<MouseEvent>? MouseMove;
    public event EventHandler<MouseEvent>? MouseWheel;
    public event EventHandler<MouseEvent>? GotFocus;
    public event EventHandler<MouseEvent>? LostFocus;
    public event EventHandler<EventArgs>? MouseEnter;
    public event EventHandler<EventArgs>? MouseLeave;
    public event EventHandler<DragEvent>? DragEnter;
    public event EventHandler<DragEvent>? DragOver;
    public event EventHandler<EventArgs>? DragLeave;
    public event EventHandler<DragEvent>? DragDrop;
    public event EventHandler<EventArgs>? Resize;

    public int Left
    {
        get => _Left;
        set
        {
            if (_Left == value) return;
            _Left = value;
            HasToResize = true;
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
                HasToResize = true;
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
                HasToResize = true;
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
                HasToResize = true;
            }
        }
    }
    public int Right
    {
        get => Width + Left;
    }
    public int Bottom
    {
        get => Height + Top;
    }

    public int AbsoluteLeft => (ParentControl?.AbsoluteLeft ?? 0) + Left;
    public int AbsoluteTop => (ParentControl?.AbsoluteTop ?? 0) + Top;
    public int AbsoluteRight
    {
        get
        {
            return AbsoluteLeft + Width;
        }
    }
    public int AbsoluteBottom => AbsoluteTop + Height;

    public bool MouseIsDown { get; private set; }

    public void Invalidate()
    {
        HasToDraw = true;
        foreach (var control in Controls)
        {
            control.Invalidate();
        }
    }

    public virtual void OnUpdate()
    {
        if (!Visible) return;

        if (!Loaded) OnLoad();

        Update?.Invoke(this, EventArgs.Empty);
        //Debug.WriteLine($"OnUpdate {GetType().Name} {Left} {Top} {Width}x{Height}");

        if (HasToResize) OnResize();

        foreach (var control in Controls)
        {
            control.OnUpdate();
        }
    }
    public virtual void OnDraw()
    {
        Draw?.Invoke(this, EventArgs.Empty);
        //Debug.WriteLine($"OnDraw {GetType().Name} {Left} {Top} {Width}x{Height}");
        HasToDraw = false;
    }

    public virtual void OnLoad()
    {
        //Debug.WriteLine($"OnLoad {GetType().Name} {Left} {Top} {Width}x{Height}");
        Load?.Invoke(this, EventArgs.Empty);
        Loaded = true;
        Invalidate();
    }
    public virtual void OnResize()
    {
        //Debug.WriteLine($"OnResize {GetType().Name} {Left} {Top} {Width}x{Height}");
        Resize?.Invoke(this, new EventArgs());
        HasToResize = false;
        Invalidate();
    }

    public virtual void OnKeyPress(KeyPressEventArgs e)
    {
        foreach (var control in Controls)
        {
            control.OnKeyPress(e);
        }
        //Debug.WriteLine($"OnKeyPress {GetType().Name} {Left} {Top} {Width}x{Height} KeyChar: {e.KeyChar}");
        KeyPress?.Invoke(this, e);
    }
    public virtual void OnKeyUp(KeyEventArgs e)
    {
        foreach (var control in Controls)
        {
            control.OnKeyUp(e);
        }
        //Debug.WriteLine($"OnKeyUp {GetType().Name} {Left} {Top} {Width}x{Height} KeyCode: {e.KeyCode}");
        KeyUp?.Invoke(this, e);
    }
    public virtual void OnKeyDown(KeyEventArgs e)
    {
        foreach (var control in Controls)
        {
            control.OnKeyDown(e);
        }
        //Debug.WriteLine($"OnKeyDown {GetType().Name} {Left} {Top} {Width}x{Height} KeyCode: {e.KeyCode}");
        KeyDown?.Invoke(this, e);
    }

    public virtual void OnGotFocus(MouseEvent e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            if (HasFocus == false)
            {
                //Debug.WriteLine($"OnGotFocus {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus}");
                HasFocus = true;
                GotFocus?.Invoke(this, e);
            }
        }
    }
    public virtual void OnLostFocus(MouseEvent e)
    {
        foreach (var control in Controls)
        {
            var newE = new MouseEvent(control, e);
            control.OnLostFocus(newE);
        }
        if (HasFocus)
        {
            //Debug.WriteLine($"OnLostFocus {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus}");
            LostFocus?.Invoke(this, e);
            HasFocus = false;
        }
    }

    public virtual void OnMouseClick(MouseEvent e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            Click?.Invoke(this, e);
            //Debug.WriteLine($"OnMouseClick {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus} X: {e.X} Y: {e.Y} Button: {e.Button} Clicks: {e.Clicks}");
            foreach (var control in Controls)
            {
                var newE = new MouseEvent(control, e);
                control.OnMouseClick(newE);
            }
        }
    }
    public virtual void OnMouseDoubleClick(MouseEvent e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            MouseDoubleClick?.Invoke(this, e);
            //Debug.WriteLine($"OnMouseDoubleClick {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus} X: {e.X} Y: {e.Y} Button: {e.Button} Clicks: {e.Clicks}");
            foreach (var control in Controls)
            {
                var newE = new MouseEvent(control, e);
                control.OnMouseDoubleClick(newE);
            }
        }
    }
    public virtual void OnMouseUp(MouseEvent e)
    {
        MouseIsDown = false;

        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            MouseUp?.Invoke(this, e);
            //Debug.WriteLine($"OnMouseUp {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus} X: {e.X} Y: {e.Y} Button: {e.Button} Clicks: {e.Clicks}");
            foreach (var control in Controls)
            {
                var newE = new MouseEvent(control, e);
                control.OnMouseUp(newE);
            }
        }
    }
    public virtual void OnMouseDown(MouseEvent e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            MouseIsDown = true;
            OnGotFocus(e);
            MouseDown?.Invoke(this, e);
            //Debug.WriteLine($"OnMouseDown {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus} X: {e.X} Y: {e.Y} Button: {e.Button} Clicks: {e.Clicks}");
            foreach (var control in Controls)
            {
                var newE = new MouseEvent(control, e);
                control.OnMouseDown(newE);
            }
        }
        else
        {
            MouseIsDown = false;
            OnLostFocus(e);
        }
    }
    public virtual void OnMouseMove(MouseEvent e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            OnMouseEnter(e);
            MouseMove?.Invoke(this, e);
            //Debug.WriteLine($"OnMouseMove {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus} X: {e.X} Y: {e.Y} Button: {e.Button} Clicks: {e.Clicks}");
            foreach (var control in Controls)
            {
                var newE = new MouseEvent(control, e);
                control.OnMouseMove(newE);
            }
        }
        else
        {
            OnMouseLeave(e);
        }
    }
    public virtual void OnMouseWheel(MouseEvent e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            MouseWheel?.Invoke(this, e);
            //Debug.WriteLine($"OnMouseWheel {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus} X: {e.X} Y: {e.Y} Button: {e.Button} Clicks: {e.Clicks} Delta: {e.Delta}");
            foreach (var control in Controls)
            {
                var newE = new MouseEvent(control, e);
                control.OnMouseWheel(newE);
            }
        }
    }
    public virtual void OnMouseEnter(EventArgs e)
    {
        if (!MouseOver)
        {
            MouseOver = true;
            MouseEnter?.Invoke(this, e);
            //Debug.WriteLine($"OnMouseEnter {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus}");
        }
    }
    public virtual void OnMouseLeave(EventArgs e)
    {
        MouseIsDown = false;

        if (MouseOver)
        {
            MouseOver = false;

            foreach (var control in Controls)
            {
                if (control.MouseOver)
                {
                    control.OnMouseLeave(e);
                }
            }

            MouseLeave?.Invoke(this, e);
            //Debug.WriteLine($"OnMouseLeave {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus}");
        }
    }

    public virtual void OnDragEnter(DragEvent e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            DragEntered = true;
            DragEnter?.Invoke(this, e);
            //Debug.WriteLine($"OnDragEnter {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus} X: {e.X} Y: {e.Y} Data: {e.Data?.GetType().Name ?? "null"}");
        }
    }
    public virtual void OnDragOver(DragEvent e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            if (!DragEntered)
            {
                OnDragEnter(e);
            }

            foreach (var control in Controls)
            {
                var newE = new DragEvent(control, e);
                if (control.Left <= e.X && e.X <= control.Right &&
                    control.Top <= e.Y && e.Y <= control.Bottom)
                {
                    control.OnDragOver(newE);
                }
                else
                {
                    if (control.DragEntered)
                    {
                        control.OnDragLeave(newE);
                    }
                }
            }

            DragOver?.Invoke(this, e);
            //Debug.WriteLine($"OnDragOver {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus} X: {e.X} Y: {e.Y} Data: {e.Data?.GetType().Name ?? "null"}");
        }
    }
    public virtual void OnDragLeave(EventArgs e)
    {
        DragEntered = false;

        foreach (var control in Controls)
        {
            if (control.DragEntered)
            {
                control.OnDragLeave(e);
            }
        }

        DragLeave?.Invoke(this, e);
        //Debug.WriteLine($"OnDragLeave {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus}");
    }
    public virtual void OnDragDrop(DragEvent e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            DragEntered = false;

            foreach (var control in Controls)
            {
                if (control.Left < e.X && e.X < control.Right &&
                    control.Top < e.Y && e.Y < control.Bottom)
                {
                    var newE = new DragEvent(control, e);
                    control.OnDragDrop(newE);
                }
            }
            DragDrop?.Invoke(this, e);
            //Debug.WriteLine($"OnDragDrop {GetType().Name} {Left} {Top} {Width}x{Height} MouseOver: {MouseOver} MouseIsDown: {MouseIsDown} HasFocus: {HasFocus} X: {e.X} Y: {e.Y} Data: {e.Data?.GetType().Name ?? "null"}");
        }
    }

    public virtual void OnDispose()
    {
    }

    public IEnumerable<GraphicsLayer> GetAllCanvasLayers()
    {
        if (!Visible) yield break;

        if (HasToDraw)
        {
            OnDraw();
        }

        foreach (var layer in GraphicsLayers)
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
        GraphicsLayers.Dispose();
        Controls.Dispose();
        GC.SuppressFinalize(this);
    }

}