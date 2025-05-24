using System.Diagnostics;
using System.Text.Json;
using VideoEditorD3D.FFMpeg.Helpers;
using VideoEditorD3D.FFMpeg.Json;

namespace VideoEditorD3D.FFMpeg;

public static class FFProbeProxy
{
    public static FFProbeRapport? GetRapport(string fullName)
    {
        var arguments = $" -v error -show_format -show_streams -print_format json \"{fullName}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = FFExecutebles.FFProbe.FullName,
                WorkingDirectory = FFExecutebles.FFProbe.Directory?.FullName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        string json = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        return JsonSerializer.Deserialize<FFProbeRapport>(json);
    }
}

