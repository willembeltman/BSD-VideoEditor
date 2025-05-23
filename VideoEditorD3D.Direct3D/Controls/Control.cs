﻿using System.Drawing;
using System.Windows.Forms;
using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Drawing;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Direct3D.Controls;

public class Control : IDisposable, IComparable
{
    private int OldLayer;
    private int _Layer = 0;
    public int Layer
    {
        get => _Layer;
        set
        {
            if (_Layer == value) return;
            _Layer = value;
            ParentControl!.Controls.Sort();
        }
    }
    public void BringToFront()
    {
        OldLayer = Layer;
        Layer = ParentControl.Controls.Max(a => Layer) + 1;
    }
    public void RestoreFromFront()
    {
        Layer = OldLayer;
    }
    public int CompareTo(object? obj)
    {
        ArgumentNullException.ThrowIfNull(obj);
        return ((Control)obj).Layer - Layer;
    }

    private int _Left = 0;
    private int _Top = 0;
    private int _Width = 480;
    private int _Height = 640;

    public IApplicationForm ApplicationForm { get; }
#nullable disable
    public Forms.Form ParentForm { get; internal set; }
    public Control ParentControl { get; internal set; }
#nullable enable
    public ControlCollection Controls { get; protected set; }
    public GraphicsLayerCollection GraphicsLayers { get; }

    public bool HasFocus { get; private set; }
    public bool Loaded { get; private set; } = false;
    public bool Dirty { get; private set; } = true;
    public bool MouseEntered { get; private set; } = false;
    public virtual bool Visible { get; set; } = true;

    public Control(IApplicationForm applicationForm)
    {
        Controls = new ControlCollection(this);
        ApplicationForm = applicationForm;
        GraphicsLayers = new GraphicsLayerCollection(this);
    }

    public event EventHandler<KeyPressEventArgs>? KeyPress;
    public event EventHandler<KeyEventArgs>? KeyUp;
    public event EventHandler<KeyEventArgs>? KeyDown;
    public event EventHandler<MouseEventArgs>? MouseClick;
    public event EventHandler<MouseEventArgs>? MouseDoubleClick;
    public event EventHandler<MouseEventArgs>? MouseUp;
    public event EventHandler<MouseEventArgs>? MouseDown;
    public event EventHandler<MouseEventArgs>? MouseMove;
    public event EventHandler<MouseEventArgs>? MouseWheel;
    public event EventHandler<MouseEventArgs>? GotFocus;
    public event EventHandler<MouseEventArgs>? LostFocus;
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

    public bool DragEntered { get; private set; }

    public void Invalidate()
    {
        //foreach (var control in Controls)
        //{
        //    control.Invalidate();
        //}
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
        Resize?.Invoke(this, new EventArgs());
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



    public virtual void OnGotFocus(MouseEventArgs e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            if (HasFocus == false)
            {
                HasFocus = true;
                GotFocus?.Invoke(this, e);
            }
        }
    }
    public virtual void OnLostFocus(MouseEventArgs e)
    {
        foreach (var control in Controls)
        {
            var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);
            control.OnLostFocus(newE);
        }
        if (HasFocus)
        {
            LostFocus?.Invoke(this, e);
            HasFocus = false;
        }
    }

    public virtual void OnMouseClick(MouseEventArgs e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            foreach (var control in Controls)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);
                if (control.Left < e.X && e.X < control.Right &&
                    control.Top < e.Y && e.Y < control.Bottom)
                {
                    control.OnMouseClick(newE);
                    break;
                }
            }
            MouseClick?.Invoke(this, e);
        }
    }

    public virtual void OnMouseDoubleClick(MouseEventArgs e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            foreach (var control in Controls)
            {
                if (control.Left < e.X && e.X < control.Right &&
                    control.Top < e.Y && e.Y < control.Bottom)
                {
                    var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);
                    control.OnMouseDoubleClick(newE);
                    break;
                }
            }
            MouseDoubleClick?.Invoke(this, e);
        }
    }
    public virtual void OnMouseUp(MouseEventArgs e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
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
    }
    public virtual void OnMouseDown(MouseEventArgs e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            foreach (var control in Controls)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);
                if (control.Left < e.X && e.X < control.Right &&
                    control.Top < e.Y && e.Y < control.Bottom)
                {
                    control.OnMouseDown(newE);
                }
                else
                {
                    control.OnLostFocus(newE);
                }
            }
            OnGotFocus(e);
            MouseDown?.Invoke(this, e);
        }
    }
    public virtual void OnMouseMove(MouseEventArgs e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            if (!MouseEntered)
            {
                OnMouseEnter(e);
            }

            foreach (var control in Controls)
            {
                var newE = new MouseEventArgs(e.Button, e.Clicks, e.X - control.Left, e.Y - control.Top, e.Delta);
                if (control.Left <= e.X && e.X <= control.Right &&
                    control.Top <= e.Y && e.Y <= control.Bottom)
                {
                    control.OnMouseMove(newE);
                }
                else
                {
                    if (control.MouseEntered)
                    {
                        control.OnMouseLeave(newE);
                    }
                }
            }

            MouseMove?.Invoke(this, e);
        }
    }
    public virtual void OnMouseWheel(MouseEventArgs e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
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
    }
    public virtual void OnMouseEnter(EventArgs e)
    {
        MouseEntered = true;
        MouseEnter?.Invoke(this, e);
    }
    public virtual void OnMouseLeave(EventArgs e)
    {
        MouseEntered = false;

        foreach (var control in Controls)
        {
            if (control.MouseEntered)
            {
                control.OnMouseLeave(e);
            }
        }

        MouseLeave?.Invoke(this, e);
    }

    public virtual void OnDragEnter(DragEvent e)
    {
        if (0 <= e.X && e.X <= Width &&
            0 <= e.Y && e.Y <= Height)
        {
            DragEntered = true;
            DragEnter?.Invoke(this, e);
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
                //var newE = new DragEventArgs(e.Data, e.KeyState, e.X - control.Left, e.Y - control.Top, e.AllowedEffect, e.Effect, e.DropImageType, e.Message, e.MessageReplacementToken);
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
                    //var newE = new DragEventArgs(e.Data, e.KeyState, e.X - control.Left, e.Y - control.Top, e.AllowedEffect, e.Effect, e.DropImageType, e.Message, e.MessageReplacementToken);
                    control.OnDragDrop(newE);
                }
            }
            DragDrop?.Invoke(this, e);
        }
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