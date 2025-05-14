using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Application.Controls;
using VideoEditorD3D.Direct3D.Interfaces;
using VideoEditorD3D.Direct3D.Forms;
using VideoEditorD3D.Direct3D.Controls;

namespace VideoEditorD3D.Application.Forms;

public class MainForm : Form
{
    private readonly MenuStrip MenuStrip;
    private readonly DisplayControl DisplayControl;
    private readonly TimelineControl TimelineControl;
    private readonly PropertiesControl PropertiesControl;
    private readonly FpsControl FpsControl;
    private readonly ApplicationContext ApplicationContext;

    private int TimelineHeight = 200;
    private int PropertiesWidth = 320;
    private bool IsMovingX;
    private bool IsMovingY;

    public MainForm(ApplicationContext applicationContext, IApplicationForm applicationForm) : base(applicationForm)
    {
        ApplicationContext = applicationContext;

        BackColor = new RawColor4(0.125f, 0.25f, 0.5f, 1);

        MenuStrip = new MenuStrip(applicationForm);
        MenuStrip.BackColor = new RawColor4(0.5f, 0.5f, 0.5f, 1);
        Controls.Add(MenuStrip);

        var fileMenu = new MenuStripItem(ApplicationForm, "File");
        MenuStrip.Items.Add(fileMenu);
        fileMenu.Items.Add(new MenuStripItem(ApplicationForm, "New"));
        fileMenu.Items.Add(new MenuStripItem(ApplicationForm, "Open"));
        fileMenu.Items.Add(new MenuStripItem(ApplicationForm, "Save"));
        fileMenu.Items.Add(new MenuStripItem(ApplicationForm, "Save As"));
        fileMenu.Items.Add(new MenuStripItem(ApplicationForm, "Exit"));

        var editMenu = new MenuStripItem(ApplicationForm, "Edit");
        MenuStrip.Items.Add(editMenu);
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, "Undo"));
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, "Redo"));
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, "Cut"));
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, "Copy"));
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, "Paste"));
        editMenu.Items.Add(new MenuStripItem(ApplicationForm, "Preferences"));

        var viewMenu = new MenuStripItem(ApplicationForm, "View");
        MenuStrip.Items.Add(viewMenu);
        viewMenu.Items.Add(new MenuStripItem(ApplicationForm, "Zoom In"));
        viewMenu.Items.Add(new MenuStripItem(ApplicationForm, "Zoom Out"));
        viewMenu.Items.Add(new MenuStripItem(ApplicationForm, "Reset Zoom"));

        var helpMenu = new MenuStripItem(ApplicationForm, "Help");
        MenuStrip.Items.Add(helpMenu);
        helpMenu.Items.Add(new MenuStripItem(ApplicationForm, "About"));
        helpMenu.Items.Add(new MenuStripItem(ApplicationForm, "Documentation"));

        DisplayControl = new DisplayControl(applicationContext, applicationForm);
        DisplayControl.BackColor = new RawColor4(0, 0, 0, 1);
        Controls.Add(DisplayControl);

        PropertiesControl = new PropertiesControl(applicationContext, applicationForm);
        PropertiesControl.BackColor = new RawColor4(0, 0, 0, 1);
        Controls.Add(PropertiesControl);

        TimelineControl = new TimelineControl(applicationContext, applicationForm);
        TimelineControl.BackColor = new RawColor4(0, 0, 0, 1);
        Controls.Add(TimelineControl);

        FpsControl = new FpsControl(applicationForm);
        Controls.Add(FpsControl);
    }
     
    public override void OnResize()
    {
        MenuStrip.Left = 0;
        MenuStrip.Top = 0;
        MenuStrip.Width = Width;
        MenuStrip.Height = 40;

        FpsControl.Left = 3;
        FpsControl.Top = 3;
        FpsControl.Width = 200;
        FpsControl.Height = 20;

        var nettowidth = Width - ApplicationConstants.Margin;
        var nettoheight = Height - ApplicationConstants.Margin - MenuStrip.Height;

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
        PropertiesControl.Left = DisplayControl.Right + ApplicationConstants.Margin;
        PropertiesControl.Width = nettowidth - linkerwidth;
        PropertiesControl.Height = topheight;

        TimelineControl.Top = DisplayControl.Bottom + ApplicationConstants.Margin;
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
            TimelineHeight = Height - e.Y - ApplicationConstants.Margin / 2;
            OnResize();
            moved = true;
        }

        if (IsMovingX)
        {
            PropertiesWidth = Width - e.X - ApplicationConstants.Margin / 2;
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
