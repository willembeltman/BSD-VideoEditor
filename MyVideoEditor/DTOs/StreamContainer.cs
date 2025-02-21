namespace MyVideoEditor.DTOs
{
    public class StreamContainer
    {
        public string? FullName { get; set; }
        public StreamInfo[]? VideoInfos { get; set; }
        public StreamInfo[]? AudioInfos { get; set; }
    }
}