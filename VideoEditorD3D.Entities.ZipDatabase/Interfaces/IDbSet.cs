using System.IO.Compression;

namespace VideoEditorD3D.Entities.ZipDatabase.Interfaces;

public interface IDbSet
{
    void WriteCache(ZipArchive zipArchive);
}