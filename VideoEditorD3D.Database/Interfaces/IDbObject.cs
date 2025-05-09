using System.IO.Compression;

namespace VideoEditorD3D.Database.Interfaces;

public interface IDbObject
{
    void WriteCache(ZipArchive zipArchive);
}