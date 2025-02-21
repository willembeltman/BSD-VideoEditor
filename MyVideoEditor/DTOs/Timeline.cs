using MyVideoEditor.Enums;

namespace MyVideoEditor.DTOs
{
    public class Timeline
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public List<TimelineVideo> TimelineVideos { get; set; } = new List<TimelineVideo>();
        public List<TimelineAudio> TimelineAudios { get; set; } = new List<TimelineAudio>();

        public Size Resolution { get; set; }
        public long FpsBase { get; set; }
        public long FpsDivider { get; set; }

        public double CurrentTime { get; set; }
        public double DisplayStart { get; set; }
        public double DisplayLength { get; set; }
        public double TotalLength { get; set; }

        public TimelineToolsEnums Tool { get; set; }
    }
}