using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Direct3D.Collections;
using VideoEditorD3D.Direct3D.Controls.Templates;

namespace VideoEditorD3D.Direct3D.Controls;

public class MenuStrip : ForeBorderBackControl
{
    public ObservableArrayCollection<MenuStripItem> Items { get; }
    public bool Opened { get; internal set; }

    public MenuStrip(Forms.Form parentForm)
    {
        ParentForm = parentForm;

        Items = new ObservableArrayCollection<MenuStripItem>();
        Items.Added += (sender, item) =>
        {
            item.MenuStrip = this;
            item.Resize += (sender, item) => { PerformLayout(); };
            Controls.Add(item);
        };
        Items.Removed += (sender, item) =>
        {
            Controls.Remove(item);
        };
        BorderSize = 0;

        Resize += MenuStrip_Resize;
        ParentForm.Resize += ParentForm_Resize;
    }

    private void ParentForm_Resize(object? sender, EventArgs e)
    {
        Left = 0;
        Top = 0;
        Width = ParentForm.Width;
    }

    private void MenuStrip_Resize(object? sender, EventArgs e)
    {
        ParentForm.OnResize();
        foreach (var item in Items)
        {
            //item.PerformLayout();
            item.InvalidateAllChildren();
        }
    }

    private void PerformLayout()
    {
        //Width = Items.Sum(a => a.Width);
        Height = Items.Max(a => a.Height) + 1;

        int x = 1;
        foreach (var item in Items)
        {
            item.Left = x;
            item.Top = 1;
            x += item.Width;
        }
        Invalidate();
    }

    public void CloseAll()
    {
        Opened = false;
        foreach (var item in Items)
        {
            item.Close();
        }
    }

    public event EventHandler<RawColor4>? MenuBackColorChanged;
    private RawColor4 _MenuBackColor = new RawColor4(0.25f, 0.25f, 0.25f, 1);
    public RawColor4 MenuBackColor
    {
        get => _MenuBackColor;
        set
        {
            if (_MenuBackColor.Equals(value)) return;
            _MenuBackColor = value;
            MenuBackColorChanged?.Invoke(this, _MenuBackColor);
            Invalidate();
        }
    }

    public event EventHandler<RawColor4>? NormalForeColorChanged;
    private RawColor4 _NormalForeColor = new RawColor4(1, 1, 1, 1);
    public RawColor4 NormalForeColor
    {
        get => _NormalForeColor;
        set
        {
            if (_NormalForeColor.Equals(value)) return;
            _NormalForeColor = value;
            NormalForeColorChanged?.Invoke(this, _NormalForeColor);
            Invalidate();
        }
    }

    public event EventHandler<RawColor4>? NormalBackColorChanged; 
    private RawColor4 _NormalBackColor = new RawColor4(0, 0, 0, 0);
    public RawColor4 NormalBackColor
    {
        get => _NormalBackColor;
        set
        {
            if (_NormalBackColor.Equals(value)) return;
            _NormalBackColor = value;
            NormalBackColorChanged?.Invoke(this, _NormalBackColor);
            Invalidate();
        }
    }

    public event EventHandler<RawColor4>? NormalBorderColorChanged;
    private RawColor4 _NormalBorderColor = new RawColor4(1, 1, 1, 1);
    public RawColor4 NormalBorderColor
    {
        get => _NormalBorderColor;
        set
        {
            if (_NormalBorderColor.Equals(value)) return;
            _NormalBorderColor = value;
            NormalBorderColorChanged?.Invoke(this, _NormalBorderColor);
            Invalidate();
        }
    }

    private int _NormalBorderSize = 0;
    public event EventHandler<int>? NormalBorderSizeChanged;
    public int NormalBorderSize
    {
        get => _NormalBorderSize;
        set
        {
            if (_NormalBorderSize == value) return;
            _NormalBorderSize = value;
            NormalBorderSizeChanged?.Invoke(this, NormalBorderSize);
            Invalidate();
        }
    }

    public event EventHandler<RawColor4>? MouseOverForeColorChanged;
    private RawColor4 _MouseOverForeColor = new RawColor4(1, 1, 1, 1);
    public RawColor4 MouseOverForeColor
    {
        get => _MouseOverForeColor;
        set
        {
            if (_MouseOverForeColor.Equals(value)) return;
            _MouseOverForeColor = value;
            MouseOverForeColorChanged?.Invoke(this, _MouseOverForeColor);
            Invalidate();
        }
    }

    public event EventHandler<RawColor4>? MouseOverBackColorChanged;
    private RawColor4 _MouseOverBackColor = new RawColor4(0, 0, 0, 0.5f);
    public RawColor4 MouseOverBackColor
    {
        get => _MouseOverBackColor;
        set
        {
            if (_MouseOverBackColor.Equals(value)) return;
            _MouseOverBackColor = value;
            MouseOverBackColorChanged?.Invoke(this, _MouseOverBackColor);
            Invalidate();
        }
    }

    public event EventHandler<RawColor4>? MouseOverBorderColorChanged;
    private RawColor4 _MouseOverBorderColor = new RawColor4(1, 1, 1, 0.5f);
    public RawColor4 MouseOverBorderColor
    {
        get => _MouseOverBorderColor;
        set
        {
            if (_MouseOverBorderColor.Equals(value)) return;
            _MouseOverBorderColor = value;
            MouseOverBorderColorChanged?.Invoke(this, _MouseOverBorderColor);
            Invalidate();
        }
    }

    private int _MouseOverBorderSize = 1;
    public event EventHandler<int>? MouseOverBorderSizeChanged;
    public int MouseOverBorderSize
    {
        get => _MouseOverBorderSize;
        set
        {
            if (_MouseOverBorderSize == value) return;
            _MouseOverBorderSize = value;
            MouseOverBorderSizeChanged?.Invoke(this, MouseOverBorderSize);
            Invalidate();
        }
    }

}

