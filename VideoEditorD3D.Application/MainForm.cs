using SharpDX.Mathematics.Interop;
using VideoEditorD3D.Application.Controls;
using VideoEditorD3D.Application.Controls.Controls;
using VideoEditorD3D.Application.Controls.TimelineControl;
using VideoEditorD3D.Direct3D.Controls;
using VideoEditorD3D.Direct3D.Forms;

namespace VideoEditorD3D.Application;

public class MainForm : Form
{
    public MenuStrip MenuStrip{ get; }
    public DisplayControl DisplayControl { get; }
    public TimelineControl TimelineControl{ get; }
    public ControlsControl ControlsControl{ get; }
    public PropertiesControl PropertiesControl{ get; }
    public FpsControl FpsControl{ get; }

    private int TimelineHeight = 200;
    private int PropertiesWidth = 320;
    private bool IsMovingX;
    private bool IsMovingY;

    public ApplicationState ApplicationContext { get; }

    public MainForm(ApplicationState applicationContext) 
    {
        ApplicationContext = applicationContext;
        BackColor = new RawColor4(0.125f, 0.25f, 0.5f, 1);

        DisplayControl = new DisplayControl();
        Controls.Add(DisplayControl);
        DisplayControl.BackColor = new RawColor4(0, 0, 0, 1);

        ControlsControl = new ControlsControl();
        Controls.Add(ControlsControl);
        ControlsControl.BackColor = new RawColor4(0, 0, 0, 1);
        ControlsControl.Resize += MainForm_Resize;

        PropertiesControl = new PropertiesControl();
        Controls.Add(PropertiesControl);
        PropertiesControl.BackColor = new RawColor4(0, 0, 0, 1);

        TimelineControl = new TimelineControl();
        Controls.Add(TimelineControl);
        TimelineControl.BackColor = new RawColor4(0, 0, 0, 1);

        MenuStrip = new MenuStrip(this);
        Controls.Add(MenuStrip);

        var fileMenu = new MenuStripItem("File");
        MenuStrip.Items.Add(fileMenu);
        fileMenu.Items.Add(new MenuStripItem("New"));
        fileMenu.Items.Add(new MenuStripItem("Open"));
        fileMenu.Items.Add(new MenuStripItem("Save"));
        fileMenu.Items.Add(new MenuStripItem("Save As"));
        fileMenu.Items.Add(new MenuStripItem("Exit"));

        var editMenu = new MenuStripItem("Edit");
        MenuStrip.Items.Add(editMenu);
        editMenu.Items.Add(new MenuStripItem("Undo"));
        editMenu.Items.Add(new MenuStripItem("Redo"));
        editMenu.Items.Add(new MenuStripItem("Cut"));
        editMenu.Items.Add(new MenuStripItem("Copy"));
        editMenu.Items.Add(new MenuStripItem("Paste"));
        editMenu.Items.Add(new MenuStripItem("Preferences"));

        var viewMenu = new MenuStripItem("View");
        MenuStrip.Items.Add(viewMenu);
        viewMenu.Items.Add(new MenuStripItem("Zoom In"));
        viewMenu.Items.Add(new MenuStripItem("Zoom Out"));
        viewMenu.Items.Add(new MenuStripItem("Reset Zoom"));

        var helpMenu = new MenuStripItem("Help");
        MenuStrip.Items.Add(helpMenu);
        helpMenu.Items.Add(new MenuStripItem("About"));
        helpMenu.Items.Add(new MenuStripItem("Documentation"));

        FpsControl = new FpsControl();
        Controls.Add(FpsControl);

        Update += MainForm_Update;
        Resize += MainForm_Resize;
        MouseMove += MainForm_MouseMove;
        MouseDown += MainForm_MouseDown;
        MouseUp += MainForm_MouseUp;
        MouseLeave += MainForm_MouseLeave;
    }

    public void MainForm_Update(object? sender, EventArgs e)
    {
        FpsControl.Left = Width - FpsControl.Width - 5;
        FpsControl.Top = (MenuStrip.Height - FpsControl.Height) / 2 + 1;
    }
    public void MainForm_Resize(object? sender, EventArgs e)
    {
        var nettowidth = Width - ApplicationConstants.Margin;
        var nettoheight = Height - ApplicationConstants.Margin - MenuStrip.Height - ControlsControl.Height;

        var linkerwidth = nettowidth - PropertiesWidth;
        var topheight = nettoheight - TimelineHeight;

        var timeline = ApplicationContext.Timeline;
        var screenWidthBasedOnHeight = topheight * timeline.Resolution.Width / timeline.Resolution.Height;
        var screenHeightBasedOnWidth = linkerwidth * timeline.Resolution.Height / timeline.Resolution.Width;

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

        ControlsControl.Top = DisplayControl.Bottom + ApplicationConstants.Margin;
        ControlsControl.Left = 0;
        ControlsControl.Width = Width;

        TimelineControl.Top = ControlsControl.Bottom;
        TimelineControl.Left = 0;
        TimelineControl.Width = Width;
        TimelineControl.Height = nettoheight - topheight;
    }
    public void MainForm_MouseMove(object? sender, MouseEvent e)
    {
        var moved = false;
        if (IsMovingY)
        {
            TimelineHeight = Height - e.Y - ControlsControl.Height - ApplicationConstants.Margin / 2;
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
    }
    public void MainForm_MouseDown(object? sender, MouseEvent e)
    {
        IsMovingX = MouseIsOverXResizer(e);
        IsMovingY = MouseIsOverYResizer(e);
    }
    public void MainForm_MouseUp(object? sender, MouseEvent e)
    {
        IsMovingX = false;
        IsMovingY = false;
    }
    public void MainForm_MouseLeave(object? sender, EventArgs e)
    {
        ApplicationForm.Cursor = System.Windows.Forms.Cursors.Default;
    }

    private bool MouseIsOverXResizer(MouseEvent e)
    {
        return e.X > DisplayControl.AbsoluteRight && e.X < PropertiesControl.Left;
    }
    private bool MouseIsOverYResizer(MouseEvent e)
    {
        return e.Y > DisplayControl.AbsoluteBottom && e.Y < ControlsControl.Top;
    }
}
