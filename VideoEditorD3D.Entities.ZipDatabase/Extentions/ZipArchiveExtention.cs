using System.IO.Compression;

namespace VideoEditorD3D.Entities.ZipDatabase.Extentions;

internal static class ZipArchiveExtention
{
    internal static ZipArchiveEntry GetOrCreateEntry(this ZipArchive zipArchive, string name)
    {
        var dataEntry = zipArchive.GetEntry(name);
        if (dataEntry == null)
            return zipArchive.CreateEntry(name);
        return dataEntry;
    }
}
