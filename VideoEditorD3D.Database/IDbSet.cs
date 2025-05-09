using System.IO.Compression;

namespace VideoEditorD3D.Database
{
    public interface IDbSet
    {
        void WriteCache(ZipArchive zipArchive);
    }
}