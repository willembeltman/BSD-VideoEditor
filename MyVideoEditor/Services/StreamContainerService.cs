using MyVideoEditor.DTOs;
using MyVideoEditor.Enums;
using MyVideoEditor.VideoObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoEditor.Services
{
    public class StreamContainerService
    {
        public FfmpegExecuteblesPaths FfmpegExecuteblesPaths { get; }
        public StreamInfoService StreamInfoService { get; }

        public StreamContainerService(
            FfmpegExecuteblesPaths ffmpegExecuteblesPaths, 
            StreamInfoService streamInfoService)
        {
            FfmpegExecuteblesPaths = ffmpegExecuteblesPaths;
            StreamInfoService = streamInfoService;
        }

        public StreamContainer Open(string fullName)
        {
            var startinfo = new ProcessStartInfo()
            {
                FileName = FfmpegExecuteblesPaths.FFProbe.FullName,
                WorkingDirectory = FfmpegExecuteblesPaths.FFProbe.Directory?.FullName,
                Arguments = " -v error -show_entries stream=codec_long_name,codec_type,width,height,r_frame_rate,duration,bit_rate -of csv=s=,:p=0 \"" + fullName + "\"",
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            using (var cmd = new Process() { StartInfo = startinfo })
            {
                cmd.Start();

                var streaminfos = new List<StreamInfo>();
                var reader = cmd.StandardOutput;
                var streamindex = 0;
                do
                {
                    var line = reader.ReadLine();
                    if (!string.IsNullOrEmpty(line))
                    {
                        var parameters = line.Split(new string[] { "," }, StringSplitOptions.None);
                        if (parameters.Length > 1)
                        {
                            var streaminfo = StreamInfoService.CreateStreamInfo(streamindex, parameters);
                            streaminfos.Add(streaminfo);
                            streamindex++;
                        }
                    }
                } while (!reader.EndOfStream);

                var videoInfos = streaminfos
                    .Where(a => a.CodecType == CodecTypeEnum.Video)
                    .ToArray();
                //var videoStreams = videoInfos
                //    .Select(a => new VideoStreamReader(FfmpegExecuteblesPaths, fullName, a))
                //    .ToArray();

                var audioInfos = streaminfos
                    .Where(a => a.CodecType == CodecTypeEnum.Audio)
                    .ToArray();
                //var audioStreams = audioInfos
                //    .Select(a => new AudioStreamReader(FfmpegExecuteblesPaths, fullName, a))
                //    .ToArray();

                return new StreamContainer()
                {
                    FullName = fullName,
                    VideoInfos = videoInfos,
                    AudioInfos = audioInfos
                };
            }
        }

        public StreamContainer[] Open(IEnumerable<string> files)
        {
            var filteredfiles = CheckFileType.Filter(files);
            if (!filteredfiles.Any())
                return Array.Empty<StreamContainer>();

            return filteredfiles
                .Select(a => Open(a))
                .ToArray();
        }
    }
}
