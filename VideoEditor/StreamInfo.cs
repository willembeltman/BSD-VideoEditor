using VideoEditor.Enums;
using VideoEditor.Dtos;
using VideoEditor.Static;
using VideoEditor.Types;

namespace VideoEditor;

public class StreamInfo
{
    public StreamInfo(File file, FFProbeStream stream)
    {
        File = file;
        Index = stream.index;
        CodecName = stream.codec_name;
        CodecLongName = stream.codec_long_name;
        CodecType = stream.codec_type == "video" ? CodecType.Video : CodecType.Audio;

        // Video
        Resolution = TryParse(stream.width, stream.height, out Resolution? resolution) ? resolution : null;
        Fps = TryParse(stream.avg_frame_rate, out Fps? fps) ? fps : null;

        // Audio
        Title = stream.tags?.title;
        Channels = stream.channels;
        SampleRate = FFHelpers.TryParseToInt(stream.sample_rate, out int sampleRate) ? sampleRate : null;
    }
    public static bool TryParse(int? width, int? height, out Resolution? resolution)
    {
        resolution = null;
        if (width == null || height == null) return false;
        resolution = new Resolution(width.Value, height.Value);
        return true;
    }
    public static bool TryParse(string? value, out Fps? result)
    {
        result = null;

        if (value == null) return false;

        var list = value.Split(['/'], StringSplitOptions.RemoveEmptyEntries);

        if (list.Length != 2) return false;
        if (!long.TryParse(list[0], out var @base)) return false;
        if (!long.TryParse(list[1], out var divider)) return false;

        result = new Fps(@base, divider);
        return true;
    }

    private File File { get; }
    public int Index { get; }
    public string? Title { get; }
    public string? CodecName { get; }
    public string? CodecLongName { get; }
    public CodecType CodecType { get; }

    public Resolution? Resolution { get; }
    public Fps? Fps { get; }

    public int? SampleRate { get; }
    public int? Channels { get; }

    public override string ToString()
    {
        if (CodecType == CodecType.Video)
        {
            return $"{Index} {CodecName} {Resolution}px {Fps}fps";
        }
        else
        {
            return $"{Index} {CodecName} {Title} {SampleRate}hz {Channels}ch";

        }
    }

    public bool EqualTo(object? obj)
    {
        if (!(obj is StreamInfo)) return false;

        var other = obj as StreamInfo;
        if (other == null) return false;
        if (File.FullName != other.File.FullName) return false;
        if (Index != other.Index) return false;
        if (Title != other.Title) return false;
        if (CodecName != other.CodecName) return false;
        if (CodecLongName != other.CodecLongName) return false;
        if (CodecType != other.CodecType) return false;
        if (CodecType == CodecType.Video && Resolution != other.Resolution) return false;
        if (CodecType == CodecType.Video && Fps != other.Fps) return false;
        if (CodecType == CodecType.Audio && SampleRate != other.SampleRate) return false;
        if (CodecType == CodecType.Audio && Channels != other.Channels) return false;

        return true;
    }
}