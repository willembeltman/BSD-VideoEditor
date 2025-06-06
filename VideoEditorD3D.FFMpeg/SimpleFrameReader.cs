using System.Diagnostics;
using System.IO;
using VideoEditorD3D.FFMpeg.Helpers;
using VideoEditorD3D.FFMpeg.Types;

namespace VideoEditorD3D.FFMpeg;

public class SimpleFrameReader
{
    /// <summary>
    /// Important: The fps is used solely for calculating the timestamp. 
    /// You cannot use a different fps than the one the video was recorded at! 
    /// You should either know the fps in advance or extract it using ffprobe.
    /// </summary>
    public SimpleFrameReader(string fullName, Resolution resolution, Fps fps, double startTime = 0)
    {
        FullName = fullName;
        Resolution = resolution;
        Fps = fps;
        StartTime = startTime;
    }

    private double _StartTime;
    private TimeStamp _StartTimeStamp;
    private long _StartFrameIndex;

    public string FullName { get; }
    public Fps Fps { get; }
    public Resolution Resolution { get; set; }

    public double StartTime
    {
        get => _StartTime;
        set
        {
            _StartTime = value;
            _StartTimeStamp = new TimeStamp(value);
            _StartFrameIndex = Fps.ConvertTimeToIndex(value);
        }
    }
    public TimeStamp StartTimeStamp
    {
        get => _StartTimeStamp;
        set
        {
            _StartTimeStamp = value;
            _StartTime = value.ConvertToTime();
            _StartFrameIndex = Fps.ConvertTimeToIndex(_StartTime);
        }
    }
    public long StartFrameIndex
    {
        get => _StartFrameIndex;
        set
        {
            _StartFrameIndex = value;
            _StartTime = Fps.ConvertIndexToTime(value);
            _StartTimeStamp = new TimeStamp(_StartTime);
        }
    }

    public IEnumerable<Frame> GetEnumerable()
    {
        var arguments = $"-i \"{FullName}\" " +
                        $"-ss {StartTimeStamp} " +
                        $"-s {Resolution.Width}x{Resolution.Height} " +
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

        using var process = Process.Start(processStartInfo) ?? throw new Exception("Cannot create process");
        using var stream = process.StandardOutput.BaseStream;

        var currentFrameIndex = StartFrameIndex;
        while (Read(stream, out var frame, ref currentFrameIndex))
        {
            yield return frame;
        }
    }

    private bool Read(Stream stream, out Frame frame, ref long currentFrameIndex)
    {
        var currentTime = Fps.ConvertIndexToTime(currentFrameIndex);

        frame = new Frame(Resolution, currentFrameIndex, currentTime);

        var endOfVideo = false;
        var read = 0;
        while (read < Resolution.ByteLength && !endOfVideo)
        {
            var partialread = stream.Read(frame.Buffer, read, Resolution.ByteLength - read);
            read += partialread;
            endOfVideo = partialread <= 0;
        }

        if (endOfVideo)
        {
            frame.Dispose();
            return false;
        }

        currentFrameIndex++;
        return true;
    }
}



