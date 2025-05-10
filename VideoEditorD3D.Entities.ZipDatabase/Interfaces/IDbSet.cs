using System.IO.Compression;

namespace VideoEditorD3D.Entities.ZipDatabase.Interfaces;

internal interface IDbSet
{
    void WriteCache(ZipArchive zipArchive);
}