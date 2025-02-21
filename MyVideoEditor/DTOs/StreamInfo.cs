using MyVideoEditor.Enums;

namespace MyVideoEditor.DTOs
{
    public class StreamInfo
    {
        public int Index { get; set; }
        public string? Codec { get; set; }
        public string? CodecTypeString { get; set; }
        public CodecTypeEnum CodecType => CodecTypeString == "video" ? CodecTypeEnum.Video : CodecTypeEnum.Audio;
        public double? Duration { get; set; }
        public int? Bitrate { get; set; }
        public Size? Resolution { get; set; }
        public long? FramerateBase { get; set; }
        public long? FramerateDivider { get; set; }
        public double? Framerate => FramerateBase / FramerateDivider;
    }
}