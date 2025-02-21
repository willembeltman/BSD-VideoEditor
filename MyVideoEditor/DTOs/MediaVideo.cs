using MyVideoEditor.VideoObjects;
using System.Text.Json.Serialization;

namespace MyVideoEditor.DTOs
{
    public class MediaVideo
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public Media? Media { get; set; }
        public StreamInfo? Stream { get; set; }
        [JsonIgnore]
        public VideoStreamReader? VideoStream { get; set; }
    }
}