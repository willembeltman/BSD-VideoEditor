using System.Collections;
using System.Diagnostics;
using VideoEditorD3D.FFMpeg.CLI.Helpers;

namespace VideoEditorD3D.FFMpeg.CLI
{
    public class AudioFrameReader : IEnumerable<AudioFrame>
    {
        public AudioFrameReader(string fullName, int sampleRate, int channels, double startTime)
        {
            FullName = fullName;
            SampleRate = sampleRate;
            Channels = channels;
            StartTime = startTime;
        }

        public string FullName { get; }
        public int SampleRate { get; }
        public int Channels { get; }
        public double StartTime { get; }


        public IEnumerable<AudioFrame> GetEnumerable()
        {
            var ffmpegArgs = $"-i \"{FullName}\" -vn -f s16le -ac {Channels} -ar {SampleRate} -";

            var processStartInfo = new ProcessStartInfo
            {
                FileName = FFExecutebles.FFMpeg.FullName,
                WorkingDirectory = FFExecutebles.FFMpeg.Directory!.FullName,
                Arguments = ffmpegArgs,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(processStartInfo);
            if (process == null) throw new Exception("Cannot start FFmpeg");

            using var stream = process.StandardOutput.BaseStream;

            var index = 0L;
            while (true)
            {
                var cliptime = Convert.ToDouble(index) / SampleRate;
                var frame = new AudioFrame(Channels, index, cliptime);

                int bytesRead = stream.Read(frame.Buffer, 0, frame.BufferSize);
                if (bytesRead == 0) yield break;

                yield return frame;

                index++;
            }
        }

        public IEnumerator<AudioFrame> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}