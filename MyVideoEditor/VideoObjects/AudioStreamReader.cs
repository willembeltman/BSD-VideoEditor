using MyVideoEditor.DTOs;

namespace MyVideoEditor.VideoObjects
{
    public class AudioStreamReader
    {
        public AudioStreamReader(FfmpegExecuteblesPaths ffmpegExecuteblesPaths, string fullName, StreamInfo streamInfo)
        {
            FfmpegExecuteblesPaths = ffmpegExecuteblesPaths;
            FullName = fullName;
            StreamInfo = streamInfo;
        }

        public FfmpegExecuteblesPaths FfmpegExecuteblesPaths { get; }
        public string FullName { get; }
        public StreamInfo StreamInfo { get; }
    }
}