using System.IO.Compression;

namespace VideoEditorD3D.Entities.ZipDatabase.Interfaces;

public interface IDbSet
{
    string TypeName { get; }

    void LoadCache(ZipArchive zipArchive);
    void WriteCache(ZipArchive zipArchive);
}