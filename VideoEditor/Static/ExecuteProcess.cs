using System.Diagnostics;

namespace VideoEditor.Static
{
    public static class ExecuteProcess
    {
        public static string ReadAllText(string executebleFullName, string? workingDirectory, string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executebleFullName,
                    WorkingDirectory = workingDirectory,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
        public static IEnumerable<string> ReadLines(string executebleFullName, string? workingDirectory, string arguments)
        {
            var startinfo = new ProcessStartInfo()
            {
                FileName = executebleFullName,
                WorkingDirectory = workingDirectory,
                Arguments = arguments,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };

            using (var cmd = new Process() { StartInfo = startinfo })
            {
                cmd.Start();

                var reader = cmd.StandardOutput;
                do
                {
                    var line = reader.ReadLine();
                    if (line != null)
                    {
                        yield return line;
                    }
                } while (!reader.EndOfStream);
            }
        }
    }
}
