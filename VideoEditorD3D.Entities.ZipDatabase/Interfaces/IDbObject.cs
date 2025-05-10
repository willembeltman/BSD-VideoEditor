using System.IO.Compression;

namespace VideoEditorD3D.Entities.ZipDatabase.Interfaces;

internal interface IDbObject
{
    void WriteCache(ZipArchive zipArchive);
}