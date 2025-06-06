using VideoEditorD3D.FFMpeg.CLI.Enums;
using VideoEditorD3D.FFMpeg.CLI.Helpers;
using VideoEditorD3D.FFMpeg.CLI.FFProbeJson;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.FFMpeg.CLI;

public class MediaStreamInfo
{
    public MediaStreamInfo(MediaContainerInfo file, FFProbeStream stream)
    {
        File = file;
        Index = stream.index;
        CodecName = stream.codec_name;
        CodecLongName = stream.codec_long_name;
        CodecType = stream.codec_type == "video" ? CodecType.Video : CodecType.Audio;
        Title = stream.tags?.title;

        // Video
        Resolution = Types.Resolution.TryParse(stream.width, stream.height, out Resolution resolution) ? resolution : null;
        Fps = Types.Fps.TryParse(stream.avg_frame_rate, out Fps fps) ? fps : null;

        // Audio
        Channels = stream.channels;
        SampleRate = FFInt.TryParse(stream.sample_rate, out int sampleRate) ? sampleRate : null;
    }

    public MediaContainerInfo File { get; }
    public int Index { get; }
    public string? Title { get; }
    public string? CodecName { get; }
    public string? CodecLongName { get; }
    public CodecType CodecType { get; }

    public Resolution? Resolution { get; }
    public Fps? Fps { get; }

    public int? SampleRate { get; }
    public int? Channels { get; }

    public VideoFrameReader OpenFrameReader(double startTime = 0)
    {
        if (CodecType != CodecType.Video)
            throw new InvalidOperationException("Reading of video frames is not permitted on a audio stream.");
        if (Fps == null)
            throw new InvalidOperationException("Fps value is not known for this stream. Unable to read.");
        if (Resolution == null)
            throw new InvalidOperationException("Resolution is not known for this stream. Unable to read.");

        return new VideoFrameReader(File.FullName, Resolution.Value, Fps.Value, startTime);
    }

    public AudioFrameReader OpenAudioFrameReader(double startTime = 0)
    {
        if (CodecType != CodecType.Audio)
            throw new InvalidOperationException("Reading of audio frames is not permitted on a video stream.");
        if (SampleRate == null)
            throw new InvalidOperationException("SampleRate value is not known for this audio stream. Unable to read.");
        if (Channels == null)
            throw new InvalidOperationException("The amount of channels is not known for this audio stream. Unable to read.");

        return new AudioFrameReader(File.FullName, SampleRate.Value, Channels.Value, startTime);
    }

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
        if (!(obj is MediaStreamInfo)) return false;

        var other = obj as MediaStreamInfo;
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