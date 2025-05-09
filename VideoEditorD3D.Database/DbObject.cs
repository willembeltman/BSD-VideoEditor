using System.IO.Compression;
using VideoEditorD3D.Database.Interfaces;
using VideoEditorD3D.Database.Extentions;

namespace VideoEditorD3D.Database;

public class DbObject<T> : IDbObject
    where T : class, new()
{
    public DbObject(DbContext dbContext)
    {
        dbContext.AddDbObject(this);

        Name = typeof(T).Name;
        Lock = new ReaderWriterLockSlim();
        Serializer = new BinarySerializer<T>();

        LoadCache(dbContext.ZipArchive);
    }

    private readonly string Name;
    private readonly ReaderWriterLockSlim Lock;
    private readonly BinarySerializer<T> Serializer;
    private T? Cache;
    public T Value => Cache!;

    private void LoadCache(ZipArchive zipArchive)
    {
        var dataFile = zipArchive.GetOrCreateEntry($"{Name}.data");
        using var dataStream = dataFile!.Open();
        using var dataReader = new BinaryReader(dataStream);

        if (dataStream.Position < dataStream.Length)
            Cache = Serializer.Read(dataReader);
        else 
            Cache = new T();
    }
    public void WriteCache(ZipArchive zipArchive)
    {
        var dataFile = zipArchive.GetOrCreateEntry($"{Name}.data");
        using var dataStream = dataFile!.Open();
        using var dataWriter = new BinaryWriter(dataStream);
        Serializer.Write(dataWriter, Cache!);
    }
}
