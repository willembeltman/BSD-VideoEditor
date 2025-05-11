using System.IO.Compression;

namespace VideoEditorD3D.Entities.ZipDatabase.Interfaces;

internal interface IDbObject
{
    DbContext DbContext { get; }
    void WriteCache(ZipArchive zipArchive);
}