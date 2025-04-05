using SharpDX.DXGI;
using VideoEditor.Static;
using VideoEditor.UI;

namespace VideoEditor;

public partial class MainForm : Form, IEngineForm
{
    public MainForm()
    {
        Engine = new Engine(this);

        TimelineControl = new TimelineControlDX2D(Engine);
        TimelineControl.AllowDrop = true;
        TimelineControl.BackColor = Color.Black;
        TimelineControl.Location = new Point(9, 298);
        TimelineControl.Margin = new Padding(2);
        TimelineControl.Name = "timelineControl";
        TimelineControl.Size = new Size(907, 220);
        TimelineControl.TabIndex = 3;
        Controls.Add(TimelineControl);

        DisplayControl = new DisplayControlDX2D(Engine);
        DisplayControl.BackColor = SystemColors.ControlDarkDark;
        DisplayControl.Location = new Point(8, 28);
        DisplayControl.Margin = new Padding(1);
        DisplayControl.Name = "displayControl";
        DisplayControl.Size = new Size(650, 255);
        DisplayControl.TabIndex = 4;
        Controls.Add(DisplayControl);

        PropertiesControl = new PropertiesControl(Engine);
        PropertiesControl.BackColor = SystemColors.ControlDarkDark;
        PropertiesControl.Location = new Point(672, 28);
        PropertiesControl.Margin = new Padding(1);
        PropertiesControl.Name = "propertiesControl";
        PropertiesControl.Size = new Size(244, 255);
        PropertiesControl.TabIndex = 5;
        Controls.Add(PropertiesControl);

        InitializeComponent();
    }

    public Engine Engine { get; }
    public TimelineControlDX2D TimelineControl { get; }
    public DisplayControlDX2D DisplayControl { get; }
    public PropertiesControl PropertiesControl { get; }

    int TimelineHeight { get; set; } = 200;
    int PropertiesWidth { get; set; } = 320;
    bool IsMovingX { get; set; }
    bool IsMovingY { get; set; }

    private void MainForm_Load(object sender, EventArgs e)
    {
        MainForm_Resize(sender, e);
        Engine.StartAll();

    }
    private void MainForm_Resize(object sender, EventArgs e)
    {
        var nettowidth = ClientRectangle.Width - Constants.Margin;
        var nettoheight = ClientRectangle.Height - Constants.Margin - menuStrip.Height;

        var linkerwidth = nettowidth - PropertiesWidth;
        var topheight = nettoheight - TimelineHeight;

        var screenWidthBasedOnHeight = topheight * Engine.Timeline.Resolution.Width / Engine.Timeline.Resolution.Height;
        var screenHeightBasedOnWidth = linkerwidth * Engine.Timeline.Resolution.Height / Engine.Timeline.Resolution.Width;

        if (topheight > screenHeightBasedOnWidth)
        {
            topheight = screenHeightBasedOnWidth;
        }
        if (linkerwidth > screenWidthBasedOnHeight)
        {
            linkerwidth = screenWidthBasedOnHeight;
        }

        DisplayControl.Top = menuStrip.Bottom;
        DisplayControl.Left = 0;
        DisplayControl.Width = linkerwidth;
        DisplayControl.Height = topheight;

        PropertiesControl.Top = menuStrip.Bottom;
        PropertiesControl.Left = DisplayControl.Right + Constants.Margin;
        PropertiesControl.Width = nettowidth - linkerwidth;
        PropertiesControl.Height = topheight;

        TimelineControl.Top = DisplayControl.Bottom + Constants.Margin;
        TimelineControl.Left = 0;
        TimelineControl.Width = ClientRectangle.Width;
        TimelineControl.Height = nettoheight - topheight;
    }
    private void MainForm_MouseMove(object sender, MouseEventArgs e)
    {
        var moved = false;
        if (IsMovingY)
        {
            TimelineHeight = ClientRectangle.Height - e.Y - Constants.Margin / 2;
            MainForm_Resize(sender, e);
            moved = true;
        }

        if (IsMovingX)
        {
            PropertiesWidth = ClientRectangle.Width - e.X - Constants.Margin / 2;
            MainForm_Resize(sender, e);
            moved = true;
        }

        if (moved) return;

        var overX = MouseIsOverXResizer(e);
        var overY = MouseIsOverYResizer(e);

        if (overX && overY)
        {
            Cursor = Cursors.SizeNWSE;
        }
        else if (overY)
        {
            Cursor = Cursors.SizeNS;
        }
        else if (overX)
        {
            Cursor = Cursors.SizeWE;
        }
        else
        {
            Cursor = Cursors.Default;
        }
    }
    public bool MouseIsOverXResizer(MouseEventArgs e)
    {
        return e.X > DisplayControl.Right && e.X < PropertiesControl.Left;
    }
    public bool MouseIsOverYResizer(MouseEventArgs e)
    {
        return e.Y > DisplayControl.Bottom && e.Y < TimelineControl.Top;
    }
    private void MainForm_MouseDown(object sender, MouseEventArgs e)
    {
        IsMovingX = MouseIsOverXResizer(e);
        IsMovingY = MouseIsOverYResizer(e);
    }
    private void MainForm_MouseUp(object sender, MouseEventArgs e)
    {
        IsMovingX = false;
        IsMovingY = false;
    }
    private void MainForm_MouseLeave(object sender, EventArgs e)
    {
        Cursor = Cursors.Default;
    }

    private void getFrameToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (Engine.IsPlaying)
            Engine.Stop();
        else
            Engine.Play();
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        Engine.Dispose();
    }
}
