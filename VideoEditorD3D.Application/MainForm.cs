using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Application.Controls;
using VideoEditorD3D.Application.Types;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Interfaces;

namespace VideoEditorD3D.Application;

public class MainForm : Form
{
    private readonly MenuStrip MenuStrip;
    private readonly DisplayControl DisplayControl;
    private readonly TimelineControl TimelineControl;
    private readonly PropertiesControl PropertiesControl;
    private readonly Button FpsLabel;
    private readonly ApplicationContext ApplicationContext;

    private int TimelineHeight = 200;
    private int PropertiesWidth = 320;
    private bool IsMovingX;
    private bool IsMovingY;

    public MainForm(ApplicationContext applicationContext, IApplicationForm applicationForm) : base(applicationForm)
    {
        ApplicationContext = applicationContext;

        BackColor = new RawColor4(0.125f, 0.25f, 0.5f, 1);

        MenuStrip = new MenuStrip(applicationForm, this, this);
        MenuStrip.BackColor = new RawColor4(0.5f, 0.5f, 0.5f, 1);
        Controls.Add(MenuStrip);

        var fileMenu = new MenuStripItem(ApplicationForm, this, MenuStrip, "File");
        MenuStrip.Items.Add(fileMenu);
        fileMenu.Items.Add(new MenuStripItem(ApplicationForm, this, fileMenu, "New"));
        fileMenu.Items.Add(new MenuStripItem(ApplicationForm, this, fileMenu, "Open"));
        fileMenu.Items.Add(new MenuStripItem(ApplicationForm, this, fileMenu, "Save"));
        fileMenu.Items.Add(new MenuStripItem(ApplicationForm, this, fileMenu, "Save As"));
        fileMenu.Items.Add(new MenuStripItem(ApplicationForm, this, fileMenu, "Exit"));

        var editMenu = new MenuStripItem(ApplicationForm, this, MenuStrip, "Edit");
        MenuStrip.Items.Add(editMenu);
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, this, editMenu, "Undo"));
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, this, editMenu, "Redo"));
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, this, editMenu, "Cut"));
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, this, editMenu, "Copy"));
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, this, editMenu, "Paste"));
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, this, editMenu, "Preferences"));

        var viewMenu = new MenuStripItem(ApplicationForm, this, MenuStrip, "View");
        MenuStrip.Items.Add(viewMenu);
        viewMenu.Items.Add(new MenuStripItem(ApplicationForm, this, viewMenu, "Zoom In"));
        viewMenu.Items.Add(new MenuStripItem(ApplicationForm, this, viewMenu, "Zoom Out"));
        viewMenu.Items.Add(new MenuStripItem(ApplicationForm, this, viewMenu, "Reset Zoom"));

        var helpMenu = new MenuStripItem(ApplicationForm, this, MenuStrip, "Help");
        MenuStrip.Items.Add(helpMenu);
        helpMenu.Items.Add(new MenuStripItem(ApplicationForm, this, helpMenu, "About"));
        helpMenu.Items.Add(new MenuStripItem(ApplicationForm, this, helpMenu, "Documentation"));


        DisplayControl = new DisplayControl(applicationContext, applicationForm, this, this);
        DisplayControl.BackColor = new RawColor4(0, 0, 0, 1);
        Controls.Add(DisplayControl);

        PropertiesControl = new PropertiesControl(applicationContext, applicationForm, this, this);
        PropertiesControl.BackColor = new RawColor4(0, 0, 0, 1);
        Controls.Add(PropertiesControl);

        TimelineControl = new TimelineControl(applicationContext, applicationForm, this, this);
        TimelineControl.BackColor = new RawColor4(0, 0, 0, 1);
        Controls.Add(TimelineControl);

        FpsLabel = new Button(applicationForm, this, this);
        FpsLabel.Left = 3;
        FpsLabel.Top = 3;
        FpsLabel.Width = 200;
        FpsLabel.Height = 20;
        FpsLabel.BackColor = new RawColor4(0, 0, 0, 0.5f);
        FpsLabel.BorderColor = new RawColor4(1, 1, 1, 1);
        FpsLabel.ForeColor = new RawColor4(1, 1, 1, 1);
        FpsLabel.BorderSize = 1;
        FpsLabel.FontSize = 6f;
        FpsLabel.Font = "Ebrima";
        Controls.Add(FpsLabel);
    }

    public override void OnUpdate()
    {
        FpsLabel.Text = $"{ApplicationForm.Timers.FpsTimer.Fps}fps   {ApplicationForm.Timers.OnUpdateTimer.Time * 1000:F3}ms   {ApplicationForm.Timers.RenderToGpuTimer.Time * 1000:F3}ms";
        base.OnUpdate();
    }
    public override void OnResize()
    {
        MenuStrip.Left = 0;
        MenuStrip.Top = 0;
        MenuStrip.Width = Width;
        MenuStrip.Height = 40;

        //var marge = 10;
        //var propertiesWidth = 240;
        //var timelineHeight = 240;

        //DisplayControl.Top = MenuStrip.Bottom + marge;
        //DisplayControl.Left = marge;
        //DisplayControl.Width = Width - propertiesWidth - marge * 3;
        //DisplayControl.Height = Height - MenuStrip.Bottom - timelineHeight - marge * 3;

        //PropertiesControl.Top = MenuStrip.Bottom + marge;
        //PropertiesControl.Left = marge + Width - propertiesWidth - marge * 3 + marge;
        //PropertiesControl.Width = propertiesWidth;
        //PropertiesControl.Height = Height - MenuStrip.Bottom - timelineHeight - marge * 3;

        //TimelineControl.Top = marge + Height - timelineHeight - marge * 3 + marge;
        //TimelineControl.Left = marge;
        //TimelineControl.Width = Width - marge * 2;
        //TimelineControl.Height = timelineHeight;

        var nettowidth = Width - Constants.Margin;
        var nettoheight = Height - Constants.Margin - MenuStrip.Height;

        var linkerwidth = nettowidth - PropertiesWidth;
        var topheight = nettoheight - TimelineHeight;

        var screenWidthBasedOnHeight = topheight * ApplicationContext.Timeline.Resolution.Width / ApplicationContext.Timeline.Resolution.Height;
        var screenHeightBasedOnWidth = linkerwidth * ApplicationContext.Timeline.Resolution.Height / ApplicationContext.Timeline.Resolution.Width;

        if (topheight > screenHeightBasedOnWidth)
        {
            topheight = screenHeightBasedOnWidth;
        }
        if (linkerwidth > screenWidthBasedOnHeight)
        {
            linkerwidth = screenWidthBasedOnHeight;
        }

        DisplayControl.Top = MenuStrip.Bottom;
        DisplayControl.Left = 0;
        DisplayControl.Width = linkerwidth;
        DisplayControl.Height = topheight;

        PropertiesControl.Top = MenuStrip.Bottom;
        PropertiesControl.Left = DisplayControl.Right + Constants.Margin;
        PropertiesControl.Width = nettowidth - linkerwidth;
        PropertiesControl.Height = topheight;

        TimelineControl.Top = DisplayControl.Bottom + Constants.Margin;
        TimelineControl.Left = 0;
        TimelineControl.Width = Width;
        TimelineControl.Height = nettoheight - topheight;
        base.OnResize();
    }
    public override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
    {
        var moved = false;
        if (IsMovingY)
        {
            TimelineHeight = Height - e.Y - Constants.Margin / 2;
            OnResize();
            moved = true;
        }

        if (IsMovingX)
        {
            PropertiesWidth = Width - e.X - Constants.Margin / 2;
            OnResize();
            moved = true;
        }

        if (moved) return;

        var overX = MouseIsOverXResizer(e);
        var overY = MouseIsOverYResizer(e);

        if (overX && overY)
        {
            ApplicationForm.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
        }
        else if (overY)
        {
            ApplicationForm.Cursor = System.Windows.Forms.Cursors.SizeNS;
        }
        else if (overX)
        {
            ApplicationForm.Cursor = System.Windows.Forms.Cursors.SizeWE;
        }
        else
        {
            ApplicationForm.Cursor = System.Windows.Forms.Cursors.Default;
        }
        base.OnMouseMove(e);
    }
    public override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
    {
        IsMovingX = MouseIsOverXResizer(e);
        IsMovingY = MouseIsOverYResizer(e);
        base.OnMouseDown(e);
    }
    public override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
    {
        IsMovingX = false;
        IsMovingY = false;
        base.OnMouseUp(e);
    }
    public override void OnMouseLeave(EventArgs e)
    {
        ApplicationForm.Cursor = System.Windows.Forms.Cursors.Default;
        base.OnMouseLeave(e);
    }


    private bool MouseIsOverXResizer(System.Windows.Forms.MouseEventArgs e)
    {
        return e.X > DisplayControl.Right && e.X < PropertiesControl.Left;
    }
    private bool MouseIsOverYResizer(System.Windows.Forms.MouseEventArgs e)
    {
        return e.Y > DisplayControl.Bottom && e.Y < TimelineControl.Top;
    }
}
