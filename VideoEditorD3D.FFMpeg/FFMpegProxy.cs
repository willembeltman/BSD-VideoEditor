using System.Diagnostics;
using VideoEditorD3D.FFMpeg.Enums;
using VideoEditorD3D.FFMpeg.Types;
using VideoEditorD3D.FFMpeg.Helpers;

namespace VideoEditorD3D.FFMpeg;

public static class FFMpegProxy
{
    public static IEnumerable<byte[]> ReadFramesAsByteArrays(string fullName, Resolution resolution, TimeStamp? startTime = null)
    {
        startTime = startTime ?? new TimeStamp();

        var arguments = $"-i \"{fullName}\" " +
                        $"-ss {startTime} " +
                        $"-pix_fmt rgba -f rawvideo -";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = FFExecutebles.FFMpeg.FullName,
            WorkingDirectory = FFExecutebles.FFMpeg.Directory!.FullName,
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using (var process = Process.Start(processStartInfo))
        {
            if (process == null) throw new Exception("Cannot start FFmpeg");
            using (var stream = process.StandardOutput.BaseStream)
            {
                int frameSize = resolution.Width * resolution.Height * 4; // rgba 4 bytes
                byte[] buffer = new byte[frameSize];

                while (ReadFrameAsByteArrays(stream, buffer))
                {
                    yield return buffer;
                }
            }
        }
    }
    private static bool ReadFrameAsByteArrays(Stream stream, byte[] buffer)
    {
        var read = 0;
        while (read < buffer.Length)
        {
            var taken = stream.Read(buffer, read, buffer.Length - read);
            if (taken <= 0) throw new Exception("Stream ended within framedata");
            read += taken;
        }
        return read == buffer.Length;
    }
    public static void WriteFramesAsByteArrays(string outputFullName, Resolution resolution, Fps fps, IEnumerable<byte[]> frames, byte crf = 23, Preset preset = Preset.medium)
    {
        var ffmpegArgs = $"-y -f rawvideo -pixel_format rgba " +
                         $"-video_size {resolution.Width}x{resolution.Height} " +
                         $"-framerate {fps} " +
                         $"-i - -c:v libx265 " +
                         $"-crf {crf} " +
                         $"-preset {Enum.GetName(preset)} " +
                         $"-r {fps} " +
                         $"\"{outputFullName}\"";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = FFExecutebles.FFMpeg.FullName,
            WorkingDirectory = FFExecutebles.FFMpeg.Directory!.FullName,
            Arguments = ffmpegArgs,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);
        if (process == null) throw new Exception("Cannot start FFmpeg");

        using var stream = process.StandardInput.BaseStream;
        int frameSize = resolution.Width * resolution.Height * 4; // rgba 4 bytes
        foreach (var frame in frames)
        {
            if (frame.Length != frameSize)
                throw new ArgumentException("Frame size mismatch!");

            stream.Write(frame, 0, frame.Length);
        }
    }

    public static IEnumerable<byte[]> ReadAudioAsByteArrays(string fullName, int channels = 2, int sampleRate = 48000)
    {
        var ffmpegArgs = $"-i \"{fullName}\" -vn -f s16le -ac {channels} -ar {sampleRate} -";

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
        int bufferSize = channels * 2; // 16-bit = 2 bytes per sample
        var buffer = new byte[bufferSize];
        while (true)
        {
            int bytesRead = stream.Read(buffer, 0, bufferSize);
            if (bytesRead == 0) yield break;
            yield return buffer;
        }
    }
    public static void WriteAudioAsByteArrays(string outputFullName, IEnumerable<byte[]> audioFrames, int channels = 2, int sampleRate = 48000, int quality = 1) // VBR quality factor (0 = best, 5 = worst)
    {
        var ffmpegArgs = $"-y -f s16le -ac {channels} -ar {sampleRate} -i - -c:a aac -q:a {quality} \"{outputFullName}\"";

        var processStartInfo = new ProcessStartInfo
        {
            FileName = FFExecutebles.FFMpeg.FullName,
            WorkingDirectory = FFExecutebles.FFMpeg.Directory!.FullName,
            Arguments = ffmpegArgs,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);
        if (process == null) throw new Exception("Cannot start FFmpeg");

        using var stream = process.StandardInput.BaseStream;
        foreach (var frame in audioFrames)
        {
            stream.Write(frame, 0, frame.Length);
        }
    }
    public static void MuxVideoWithMultipleAudioStreams(string outputFullName, string videoFullName, IEnumerable<string> audioFullNames)
    {
        // Basis FFmpeg argumenten voor het muxen van 1 video met meerdere audiostreams.
        var ffmpegArgs = $"-y -i \"{videoFullName}\" ";  // Voeg video toe als eerste input

        // Voeg de audio bestanden toe (meerdere bestanden kunnen worden toegevoegd)
        foreach (var audioFile in audioFullNames)
        {
            ffmpegArgs += $"-i \"{audioFile}\" ";  // Voeg elke audio stream toe
        }

        // Voeg de kopieeropties toe
        // -c:v copy voor video (geen herencoding)
        // -c:a copy voor audio (geen herencoding)
        // Voeg de audio streams in de juiste volgorde toe met -map
        ffmpegArgs += "-c:v copy ";  // Video niet her-encoderen
        int audioStreamIndex = 1;  // Start bij index 1 voor audiostreams (0 is de videostream)
        foreach (var _ in audioFullNames)
        {
            ffmpegArgs += $"-c:a:{audioStreamIndex} copy ";  // Audio niet her-encoderen
            audioStreamIndex++;
        }

        // Voeg de mapping toe voor de output bestand
        ffmpegArgs += "-map 0:v:0 ";  // Eerste video stream
        audioStreamIndex = 1;
        foreach (var _ in audioFullNames)
        {
            ffmpegArgs += $"-map {audioStreamIndex}:a:0 ";  // Voeg elke audiostream toe
            audioStreamIndex++;
        }

        // Specificeer de output bestandsnaam
        ffmpegArgs += $"\"{outputFullName}\"";

        // Start de FFmpeg process
        var processStartInfo = new ProcessStartInfo
        {
            FileName = FFExecutebles.FFMpeg.FullName,
            WorkingDirectory = FFExecutebles.FFMpeg.Directory!.FullName,
            Arguments = ffmpegArgs,
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(processStartInfo);
        if (process == null) throw new Exception("Cannot start FFmpeg");
        process.WaitForExit(); // Wacht tot het proces klaar is
    }
}