using System.Collections;
using System.Diagnostics;
using VideoEditor.Types;
namespace VideoEditor.FF;

public class FFMpeg_FrameReader : IEnumerator<byte[]?>
{
    public FFMpeg_FrameReader(
        string fullName,
        Resolution resolution,
        TimeStamp? startTime = null)
    {
        FullName = fullName;
        Resolution  = resolution;
        StartTime = startTime ?? new TimeStamp();
    }

    public string FullName { get; }
    public Resolution Resolution { get; }
    public TimeStamp StartTime { get; }
    public byte[]? Current { get; private set; }
    public Process? Process { get; private set; }

    object? IEnumerator.Current => Current;

    public void Dispose()
    {
        Process?.Dispose();
    }

    public bool MoveNext()
    {
        if (Process == null)
            Reset();

        if (Process == null) 
            return false;

        if (Current == null)
            Current = new byte[Resolution.Width * Resolution.Height * 4];

        var read = 0;
        while (read < Current.Length)
        {
            var taken = Process.StandardOutput.BaseStream.Read(Current, read, Current.Length - read);
            if (taken <= 0) return false;
            read += taken;
        }
        return true;
    }

    public void Reset()
    {
        var arguments = $"-i \"{FullName}\" " +
                        $"-ss {StartTime} " +
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

        Process = Process.Start(processStartInfo);
        Current = null;
    }
}
