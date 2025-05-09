using System.IO.Compression;

namespace VideoEditorD3D.Database
{
    public static class ZipArchiveExtention
    {
        public static ZipArchiveEntry GetOrCreateEntry(this ZipArchive zipArchive, string name)
        {
            var dataEntry = zipArchive.GetEntry(name);
            if (dataEntry == null)
                return zipArchive.CreateEntry(name);
            return dataEntry;
        }
    }
}
