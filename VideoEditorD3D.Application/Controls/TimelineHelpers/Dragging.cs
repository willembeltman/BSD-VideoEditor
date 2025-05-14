
using System.Drawing;

namespace VideoEditorD3D.Application.Controls.TimelineHelpers;

public class Dragging
{
    public bool IsDragging { get; set; }
    public Point StartPoint { get; set; }
    public TimelinePosition StartPosition { get; set; }

    internal void Set(Point startpoint, TimelinePosition? startposition)
    {
        if (startposition == null) throw new ArgumentNullException(nameof(startposition));
        IsDragging = true;
        StartPoint = startpoint;
        StartPosition = startposition.Value;
    }
}