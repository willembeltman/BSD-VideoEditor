namespace MyVideoEditor.DTOs
{
    public class Media
    {
        public Media(StreamContainer streamContainer)
        {
            StreamContainer = streamContainer;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public StreamContainer? StreamContainer { get; set; }
        public List<MediaVideo> Videos { get; set; } = new List<MediaVideo>();
        public List<MediaAudio> Audios { get; set; } = new List<MediaAudio>();
    }
}