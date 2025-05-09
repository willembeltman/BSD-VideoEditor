using System.IO.Compression;

namespace VideoEditorD3D.Database
{
    public interface IDbObject
    {
        void WriteCache(ZipArchive zipArchive);
    }
}