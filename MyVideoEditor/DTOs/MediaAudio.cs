using MyVideoEditor.VideoObjects;
using System.Text.Json.Serialization;

namespace MyVideoEditor.DTOs
{
    public class MediaAudio
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonIgnore]
        public Media? Media { get; set; }
        public StreamInfo? Stream { get; set; }
        [JsonIgnore]
        public AudioStreamReader? AudioStream { get; set; }
    }
}