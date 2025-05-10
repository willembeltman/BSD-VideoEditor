using VideoEditorD3D.FFMpeg;
using VideoEditorD3D.FFMpeg.Json;
using VideoEditorD3D.FFMpeg.Enums;
using VideoEditorD3D.FFMpeg.Helpers;

namespace VideoEditor;

public class File
{
    private File(string fullName, FFProbeRapport rapport)
    {
        if (rapport.streams == null) throw new Exception("Media container rapport has no 'streams'");
        if (rapport.format == null) throw new Exception("Media container rapport has no 'format'");

        FullName = fullName;
        AllStreams =
            rapport.streams
                .Where(a => a.codec_type == "video" || a.codec_type == "audio")
                .Select(a => new StreamInfo(this, a))
                .ToArray();
        VideoStreams =
            AllStreams
                .Where(a => a.CodecType == CodecType.Video)
                .ToArray();
        AudioStreams =
            AllStreams
                .Where(a => a.CodecType == CodecType.Audio)
                .ToArray();
        Duration = FFDouble.TryParse(rapport.format.duration, out var dur) ? dur : null;
    }

    public string FullName { get; }
    public StreamInfo[] AllStreams { get; }
    public StreamInfo[] VideoStreams { get; }
    public StreamInfo[] AudioStreams { get; }
    public double? Duration { get; }

    public static IEnumerable<File> OpenMultiple(IEnumerable<string> files)
    {
        return files
            .Select(Open)
            .Where(a => a != null)
            .Select(a => a!);
    }
    public static File? Open(string fullName)
    {
        var rapport = FFProbeProxy.GetRapport(fullName);
        if (rapport == null) return null;
        return new File(fullName, rapport);
    }

    public bool EqualTo(object? obj)
    {
        if (!(obj is File)) return false; 
        var other = obj as File;
        if (other == null) return false;
        if (FullName != other.FullName) return false;
        return true;
    }

    public override string ToString()
    {
        return $"{FullName} {Duration}s";
    }

}
