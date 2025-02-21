namespace MyVideoEditor.DTOs
{
    public class TimelineItem
    {
        public Guid GroupId { get; set; }
        public int TimelineIndex { get; set; }
        public double TimelineStartTime { get; set; }
        public double TimelineEndTime { get; set; }
        public double MediaStartTime { get; set; }
        public double MediaEndTime { get; set; }
    }
}