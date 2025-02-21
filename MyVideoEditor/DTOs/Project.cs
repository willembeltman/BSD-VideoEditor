namespace MyVideoEditor.DTOs
{
    public class Project
    {
        public List<Timeline> Timelines { get; set; } = new List<Timeline>();
        public List<Media> Medias { get; set; } = new List<Media>();

        public int Width { get; set; }
        public int Height { get; set; }
        public long FramerateBase { get; set; }
        public long FramerateDivider { get; set; }

        public Guid CurrentTimelineId { get; set; }
    }
}
