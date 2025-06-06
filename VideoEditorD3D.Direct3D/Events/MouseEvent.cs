namespace VideoEditorD3D.Direct3D.Events;

public class MouseEvent : EventArgs
{
    public MouseEvent(Controls.Control control, MouseEventArgs e)
    {
        E = e;
        X = e.X - control.Left;
        Y = e.Y - control.Top;
    }
    public MouseEvent(Controls.Control control, MouseEvent e)
    {
        E = e.E;
        X = e.X - control.Left;
        Y = e.Y - control.Top;
    }
    public MouseEventArgs E { get; }

    /// <summary>
    ///  Gets which mouse button was pressed.
    /// </summary>
    public MouseButtons Button => E.Button;

    /// <summary>
    ///  Gets the number of times the mouse button was pressed and released.
    /// </summary>
    public int Clicks => E.Clicks;

    /// <summary>
    ///  Gets the x-coordinate of a mouse click.
    /// </summary>
    public int X { get; }

    /// <summary>
    ///  Gets the y-coordinate of a mouse click.
    /// </summary>
    public int Y { get; }

    /// <summary>
    ///  Gets a signed count of the number of detents the mouse wheel has rotated.
    /// </summary>
    public int Delta => E.Delta;

    /// <summary>
    ///  Gets the location of the mouse during MouseEvent.
    /// </summary>
    public Point Location => new(X, Y);

}
