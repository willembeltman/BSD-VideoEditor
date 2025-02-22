namespace VideoEditor.Types
{
    public class Dragging
    {
        public bool IsDragging { get; set; }
        public Point StartPoint { get; set; }
        public TimelinePosition StartPosition { get; set; }
    }
}