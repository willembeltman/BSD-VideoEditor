using System.Diagnostics;
using MyVideoEditor.DTOs;

namespace MyVideoEditor.VideoObjects
{
    public class VideoStreamWriter : IDisposable
    {
        public VideoStreamWriter(DirectoryInfo FFMpegDirectory, string fullName, int frameWidth, int frameHeight, int fps = 25, int crf = 23, string preset = "medium")
        {
            FullName = fullName;
            FrameWidth = frameWidth;
            FrameHeight = frameHeight;
            Process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Path.Combine(FFMpegDirectory.FullName, "ffmpeg.exe"),
                    WorkingDirectory = FFMpegDirectory.FullName,
                    Arguments = $"-f rawvideo -pix_fmt rgb24 -s {FrameWidth}x{FrameHeight} -r {fps} -i - -c:v libx265 -crf {crf} -preset {preset} \"{FullName}\"",
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            Process.Start();
            StreamWriter = new BinaryWriter(Process.StandardInput.BaseStream);
        }

        public string FullName { get; }
        public int FrameWidth { get; }
        public int FrameHeight { get; }
        public Process Process { get; }
        public BinaryWriter StreamWriter { get; }

        public void WriteFrame(VideoFrame frame)
        {
            if (frame.Width != FrameWidth || frame.Height != FrameHeight)
                throw new Exception($"Frame size ({frame.Width}X{frame.Height}) not the same as videostream size ({FrameWidth}X{FrameHeight})");
            StreamWriter.Write(frame.FrameData);
        }

        public void Dispose()
        {
            StreamWriter?.Dispose();
            Process?.Dispose();
        }
    }
}