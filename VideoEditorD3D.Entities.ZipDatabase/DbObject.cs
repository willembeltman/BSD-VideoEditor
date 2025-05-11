using System.IO.Compression;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;
using VideoEditorD3D.Entities.ZipDatabase.Extentions;
using VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;

namespace VideoEditorD3D.Entities.ZipDatabase;

public class DbObject<T> : IDbObject
    where T : class, new()
{
    private readonly string Name;
    private readonly ReaderWriterLockSlim Lock;
    private readonly EntitySerializer<T> Serializer;
    private T? Cache;

    public DbContext DbContext { get; }

    public DbObject(DbContext dbContext)
    {
        DbContext = dbContext;
        dbContext.AddDbObject(this);

        Name = typeof(T).Name;
        Lock = new ReaderWriterLockSlim();
        Serializer = EntitySerializerCollection.GetEntitySerializer<T>(dbContext);

        LoadCache(dbContext.ZipArchive);
    }

    public T? Value
    {
        get
        {
            T? item = null;

            Lock.EnterReadLock();
            try
            {
                item = Cache!;
            }
            finally
            {
                Lock.ExitReadLock();
            }

            return item;
        }
    }


    private void LoadCache(ZipArchive zipArchive)
    {
        Lock.EnterWriteLock();
        try
        {
            var dataFile = zipArchive.GetOrCreateEntry($"{Name}.data");
            using var dataStream = dataFile!.Open();
            using var dataReader = new BinaryReader(dataStream);

            if (dataStream.Position < dataStream.Length)
                Cache = Serializer.Read(dataReader, DbContext);
            else
                Cache = new T();
        }
        finally
        {
            Lock.ExitWriteLock();
        }
    }
    public void WriteCache(ZipArchive zipArchive)
    {
        Lock.EnterReadLock();
        try
        {
            var dataFile = zipArchive.GetOrCreateEntry($"{Name}.data");
            using var dataStream = dataFile!.Open();
            using var dataWriter = new BinaryWriter(dataStream);
            Serializer.Write(dataWriter, Cache!, DbContext);
        }
        finally
        {
            Lock.ExitReadLock();
        }
    }
}
