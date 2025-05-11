namespace VideoEditorD3D.Application.Helpers;

public static class CheckFileType
{
    static string[] supportedExtensions = { ".mp4", ".avi", ".mkv", ".mov", ".wmv" };

    public static IEnumerable<string> Filter(IEnumerable<string> files)
    {
        return files
            .Where(a => Check(a));
    }
    public static bool Check(string file)
    {
        string extension = Path.GetExtension(file);
        return
            !string.IsNullOrEmpty(extension) &&
            supportedExtensions.Contains(extension.ToLower());
    }
}
