using System.IO.Compression;

namespace VideoEditorD3D.Entities.ZipDatabase.Interfaces;

public interface IDbSet
{
    string TypeName { get; }

    void WriteCache(ZipArchive zipArchive);
}