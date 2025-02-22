
namespace VideoEditor.Types
{
    public class Dragging
    {
        public bool IsDragging { get; set; }
        public Point StartPoint { get; set; }
        public TimelinePosition StartPosition { get; set; }

        internal void Set(Point startpoint, TimelinePosition? startposition)
        {
            IsDragging = true;
            StartPoint = startpoint;
            StartPosition = startposition.Value;
        }
    }
}