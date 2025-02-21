namespace MyVideoEditor.VideoObjects
{
    public class FfmpegExecuteblesPaths
    {
        public FfmpegExecuteblesPaths(string processPath, string? executeblesDirectoryName)
        {
            if (executeblesDirectoryName == null || executeblesDirectoryName.Length == 0)
                throw new Exception("ExecuteblesPath is empty");

            var processPathFileInfo = new FileInfo(processPath);

            if (processPathFileInfo.Directory == null || processPathFileInfo.Directory.Exists == false)
                throw new Exception("processPathFileInfo is empty");

            var executeblesPath = Path.Combine(processPathFileInfo.Directory.FullName, executeblesDirectoryName);
            var executeblesDirInfo = new DirectoryInfo(executeblesPath);
            if (executeblesDirInfo.Exists == false)
                throw new Exception("ExecuteblesPath does not exist");

            FFMpeg = new FileInfo(Path.Combine(executeblesDirInfo.FullName, "ffmpeg.exe"));
            FFProbe = new FileInfo(Path.Combine(executeblesDirInfo.FullName, "ffprobe.exe"));

            if (!FFMpeg.Exists)
                throw new Exception($"Cannot find ffmpeg.exe executeble in folder '{executeblesDirInfo.FullName}'");
            if (!FFProbe.Exists)
                throw new Exception($"Cannot find ffprobe.exe executeble in folder '{executeblesDirInfo.FullName}'");
        }

        public FileInfo FFMpeg { get; }
        public FileInfo FFProbe { get; }
    }
}