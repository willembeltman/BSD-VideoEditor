using System.Diagnostics;
using VideoEditor.FF;
using VideoEditor.Static;
using VideoEditor.UI;

namespace VideoEditor;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        Engine.MainForm = this;
    }

    int TimelineHeight { get; set; } = 200;
    int PropertiesWidth { get; set; } = 320;
    bool IsMovingX { get; set; }
    bool IsMovingY { get; set; }

    private void MainForm_Load(object sender, EventArgs e)
    {
        MainForm_Resize(sender, e);
        Engine.StartEngine();
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

        displayControl.Top = menuStrip.Bottom;
        displayControl.Left = 0;
        displayControl.Width = linkerwidth;
        displayControl.Height = topheight;

        propertiesControl.Top = menuStrip.Bottom;
        propertiesControl.Left = displayControl.Right + Constants.Margin;
        propertiesControl.Width = nettowidth - linkerwidth;
        propertiesControl.Height = topheight;

        timelineControl.Top = displayControl.Bottom + Constants.Margin;
        timelineControl.Left = 0;
        timelineControl.Width = ClientRectangle.Width;
        timelineControl.Height = nettoheight - topheight;
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
        return e.X > displayControl.Right && e.X < propertiesControl.Left;
    }
    public bool MouseIsOverYResizer(MouseEventArgs e)
    {
        return e.Y > displayControl.Bottom && e.Y < timelineControl.Top;
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
        Engine.StopEngine();
    }

    private void timer_Tick(object sender, EventArgs e)
    {
        propertiesControl.UpdateFps();
    }

    //private void test2ToolStripMenuItem_Click(object sender, EventArgs e)
    //{
    //    var fullname = @"D:\Willem\Videos\2025-03-07 09-43-50.mkv";
    //    using var ding = new FFMpeg_FrameReader(fullname, new Types.Resolution());

    //    var skip = 1;
    //    var index = 0;
    //    var index2 = 0;
    //    var stopwatch = Stopwatch.StartNew();

    //    //bool stop = false;
    //    //while (!stop)
    //    //{
    //    //    var frame = ding.GetFrame(index, index + skip);
    //    //    stop = frame == null;
    //    //    index += skip;

    //    //    //Thread.Sleep(10);
    //    //    if (index2 % 100 == 0)
    //    //        Debug.WriteLine($"{Convert.ToDouble(index2) / stopwatch.Elapsed.TotalSeconds}fps");
    //    //    index2++;
    //    //}

    //    var next = skip;
    //    var reader = FFMpeg.ReadFrames(fullname, new Types.Resolution());
    //    foreach (var frame in reader)
    //    {
    //        if (next == index)
    //        {
    //            next += skip;

    //            //Thread.Sleep(10);
    //            if (index2 % 100 == 0)
    //                Debug.WriteLine($"{Convert.ToDouble(index2) / stopwatch.Elapsed.TotalSeconds}fps");
    //            index2++;
    //        }
    //        index++;
    //    }
    //}
}
