using System.Diagnostics;
using MyVideoEditor.DTOs;
using MyVideoEditor.Services;

namespace MyVideoEditor.VideoObjects
{
    public class VideoStreamReader : IDisposable
    {
        public VideoStreamReader(
            FfmpegExecuteblesPaths ffmpegExecuteblesPaths, 
            string fullName, 
            StreamInfo streaminfo)
        {
            if (streaminfo.Resolution == null) throw new Exception("Resolution must be known");
            if (streaminfo.FramerateBase == null) throw new Exception("Framerate must be known");
            if (streaminfo.FramerateDivider == null) throw new Exception("Framerate must be known");

            FfmpegExecuteblesPaths = ffmpegExecuteblesPaths;
            FullName = fullName;
            StreamInfo = streaminfo;
            TimeStampService = new TimeStampService();

        }

        public FfmpegExecuteblesPaths FfmpegExecuteblesPaths { get; }
        public string FullName { get; }
        public StreamInfo StreamInfo { get; }
        public TimeStampService TimeStampService { get; }

        private long NextReadFrameIndex { get; set; }

        private Process? Process { get; set; }
        private BinaryReader? StreamReader { get; set; }

        public int FrameWidth => StreamInfo.Resolution.Value.Width;
        public int FrameHeight => StreamInfo.Resolution.Value.Height;
        public long FramerateBase => StreamInfo.FramerateBase.Value;
        public long FramerateDivider => StreamInfo.FramerateDivider.Value;

        public VideoFrame? GetFrame(long requestedFrameIndex)
        {
            if (Process == null)
            {
                StartProcess(requestedFrameIndex);
            }
            else
            {
                var verschil = requestedFrameIndex - NextReadFrameIndex;
                if (verschil < 0)
                {
                    // Opnieuw starten
                    StartProcess(requestedFrameIndex);
                }
                else if (verschil > 0)
                {
                    if (verschil < 100)
                    {
                        for (int i = 0; i < verschil; i++)
                        {
                            // Frame lezen zonder iets mee te doen
                            var dummy = ReadNextFrame(); 
                        }
                    }
                    else
                    {
                        // Te ver zoeken, opnieuw starten
                        StartProcess(requestedFrameIndex);
                    }
                }
            }

            var frame = ReadNextFrame();
            return frame;
        }


        private void StartProcess(long startframeindex)
        {
            if (Process != null || StreamReader != null)
            {
                Dispose();
            }

            string ss = TimeStampService.GetTimeStamp(startframeindex, FramerateBase, FramerateDivider);
            var path1 = FfmpegExecuteblesPaths.FFMpeg.FullName;
            var path2 = FfmpegExecuteblesPaths.FFMpeg.Directory.FullName;

            Process = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = path1,
                    WorkingDirectory = path2,
                    Arguments = $"-i \"{FullName}\" -ss {ss} -f rawvideo -pix_fmt rgb24 -",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            Process.Start();
            StreamReader = new BinaryReader(Process.StandardOutput.BaseStream);
            NextReadFrameIndex = startframeindex;
        }
        private VideoFrame? ReadNextFrame()
        {
            if (StreamReader == null) return null; // not started jet

            int frameSizeInBytes = FrameWidth * FrameHeight * 3;
            int bytesRead;
            int totalBytesRead = 0;
            byte[] frameData = new byte[frameSizeInBytes];

            while ((bytesRead = StreamReader.Read(frameData, totalBytesRead, frameData.Length - totalBytesRead)) > 0)
            {
                totalBytesRead += bytesRead;
                if (totalBytesRead == frameData.Length)
                {
                    NextReadFrameIndex++;
                    var frame = new VideoFrame(frameData, FrameWidth, FrameHeight, NextReadFrameIndex);
                    return frame;
                }
            }
            return null;
        }

        public void Dispose()
        {
            StreamReader?.Dispose();
            Process?.Dispose();
        }
    }
}