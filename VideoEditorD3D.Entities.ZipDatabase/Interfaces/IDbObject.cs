using System.IO.Compression;

namespace VideoEditorD3D.Entities.ZipDatabase.Interfaces;

public interface IDbObject
{
    void WriteCache(ZipArchive zipArchive);
}