namespace VideoEditorD3D.Direct3D.Events;

using Control = Controls.Control;

public class DragEvent : EventArgs
{
    public DragEvent(DragEventArgs dragEventArgs, int x, int y)
    {
        DragEventArgs = dragEventArgs;
        X = x;
        Y = y;
    }
    public DragEvent(Control control, DragEvent dragEvent)
    {
        DragEventArgs = dragEvent.DragEventArgs;
        X = dragEvent.X - control.Left;
        Y = dragEvent.Y - control.Top;
    }

    public DragEventArgs DragEventArgs { get; }

    /// <summary>
    ///  The <see cref="IDataObject"/> that contains the data associated
    ///  with this event.
    /// </summary>
    public IDataObject? Data => DragEventArgs.Data;

    /// <summary>
    ///  Gets the current state of the SHIFT, CTRL, and ALT keys.
    /// </summary>
    public int KeyState => DragEventArgs.KeyState;

    /// <summary>
    ///  Gets the x-coordinate of the mouse pointer.
    /// </summary>
    public int X { get; }

    /// <summary>
    ///  Gets the y-coordinate of the mouse pointer.
    /// </summary>
    public int Y { get; }

    /// <summary>
    ///  Gets which drag-and-drop operations are allowed by the originator (or source)
    ///  of the drag event.
    /// </summary>
    public DragDropEffects AllowedEffect => DragEventArgs.AllowedEffect;

    /// <summary>
    ///  Gets or sets which drag-and-drop operations are allowed by the target of the drag event.
    /// </summary>
    public DragDropEffects Effect
    {
        get => DragEventArgs.Effect;
        set => DragEventArgs.Effect = value;
    }

    /// <summary>
    ///  Gets or sets the drop description image type.
    /// </summary>
    public DropImageType DropImageType
    {
        get => DragEventArgs.DropImageType;
        set => DragEventArgs.DropImageType = value;
    }

    /// <summary>
    ///  Gets or sets the drop description text such as "Move to %1".
    /// </summary>
    /// <remarks>
    /// <para>
    /// UI coloring is applied to the text in <see cref="MessageReplacementToken"/> if used by specifying %1 in <see cref="Message"/>.
    /// </para>
    /// </remarks>
    public string? Message
    {
        get => DragEventArgs.Message;
        set => DragEventArgs.Message = value;
    }

    /// <summary>
    ///  Gets or sets the drop description text such as "Documents" when %1 is specified in <see cref="Message"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// UI coloring is applied to the text in <see cref="MessageReplacementToken"/> if used by specifying %1 in <see cref="Message"/>.
    /// </para>
    /// </remarks>
    public string? MessageReplacementToken
    {
        get => DragEventArgs.MessageReplacementToken;
        set => DragEventArgs.MessageReplacementToken = value;
    }
}
