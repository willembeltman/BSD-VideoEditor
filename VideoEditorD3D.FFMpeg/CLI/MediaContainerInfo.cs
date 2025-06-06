using VideoEditorD3D.FFMpeg.CLI.Enums;
using VideoEditorD3D.FFMpeg.CLI.Helpers;
using VideoEditorD3D.FFMpeg.CLI.Json;

namespace VideoEditorD3D.FFMpeg.CLI;

public class MediaContainerInfo
{
    private MediaContainerInfo(string fullName, FFProbeRapport rapport)
    {
        if (rapport.streams == null) throw new Exception("Media container rapport has no 'streams'");
        if (rapport.format == null) throw new Exception("Media container rapport has no 'format'");

        FullName = fullName;
        AllStreams =
            rapport.streams
                .Where(a => a.codec_type == "video" || a.codec_type == "audio")
                .Select(a => new MediaStreamInfo(this, a))
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
    public MediaStreamInfo[] AllStreams { get; }
    public MediaStreamInfo[] VideoStreams { get; }
    public MediaStreamInfo[] AudioStreams { get; }
    public double? Duration { get; }

    public static IEnumerable<MediaContainerInfo> OpenMultiple(IEnumerable<string> files)
    {
        return files
            .Select(Open)
            .Where(a => a != null)
            .Select(a => a!);
    }
    public static MediaContainerInfo? Open(string fullName)
    {
        var rapport = FFProbeProxy.GetRapport(fullName);
        if (rapport == null) return null;
        return new MediaContainerInfo(fullName, rapport);
    }

    public bool EqualTo(object? obj)
    {
        if (!(obj is MediaContainerInfo)) return false;
        var other = obj as MediaContainerInfo;
        if (other == null) return false;
        if (FullName != other.FullName) return false;
        return true;
    }

    public override string ToString()
    {
        return $"{FullName} {Duration}s";
    }

}
