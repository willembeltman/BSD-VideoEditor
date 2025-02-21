namespace VideoEditor.Static
{
    public static class FFExecutebles
    {

        public static FileInfo FFMpeg
        {
            get
            {
                var executeblesInfo = GetExecuteblesDirectoryInfo();
                var fFMpeg = new FileInfo(Path.Combine(executeblesInfo.FullName, "ffmpeg.exe"));
                if (!fFMpeg.Exists)
                    throw new Exception($"Cannot find ffmpeg.exe executeble in folder '{executeblesInfo.FullName}'");
                return fFMpeg;
            }
        }
        public static FileInfo FFProbe
        {
            get
            {
                var executeblesInfo = GetExecuteblesDirectoryInfo();
                var fFProbe = new FileInfo(Path.Combine(executeblesInfo.FullName, "ffprobe.exe"));
                if (!fFProbe.Exists)
                    throw new Exception($"Cannot find ffprobe.exe executeble in folder '{executeblesInfo.FullName}'");
                return fFProbe;
            }
        }

        private static DirectoryInfo GetExecuteblesDirectoryInfo()
        {
            var executeblesDirectoryName = "Executebles";
            if (executeblesDirectoryName == null || executeblesDirectoryName.Length == 0)
                throw new Exception("ExecuteblesPath is empty");

            var processPath = Environment.ProcessPath;
            if (processPath == null || processPath.Length == 0)
                throw new Exception("ProcessPath is empty");

            var processPathFileInfo = new FileInfo(processPath);

            if (processPathFileInfo.Directory == null || processPathFileInfo.Directory.Exists == false)
                throw new Exception("processPathFileInfo is empty");

            var executeblesPath = Path.Combine(processPathFileInfo.Directory.FullName, executeblesDirectoryName);
            var executeblesDirInfo = new DirectoryInfo(executeblesPath);
            if (executeblesDirInfo.Exists == false)
                throw new Exception("ExecuteblesPath does not exist");
            return executeblesDirInfo;
        }
    }
}