using System.IO.Compression;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;
using VideoEditorD3D.Entities.ZipDatabase.Extentions;
using VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;

namespace VideoEditorD3D.Entities.ZipDatabase;

public class DbObject<T> : IDbObject
    where T : class, IEntity, new()
{
    private readonly string Name;
    private readonly ReaderWriterLockSlim Lock;
    private readonly EntitySerializer<T> EntitySerializer;
    private readonly EntityExtender<T> EntityExtender;
    private T? Cache;

    public object DbContextObject { get; }
    public DbContext DbContext { get; }

    public DbObject(object dbContext)
    {
        DbContextObject = dbContext;
        DbContext = (dbContext as DbContext)!;
        DbContext.AddDbObject(this);

        Name = typeof(T).Name;
        Lock = new ReaderWriterLockSlim();
        EntitySerializer = EntitySerializerCollection.GetEntitySerializer<T>();
        EntityExtender = EntityExtenderCollection.GetEntityExtender<T>(DbContext);

        //LoadCache(DbContext.ZipArchive);
    }

    public T Value
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


    public void LoadCache(ZipArchive zipArchive)
    {
        Lock.EnterWriteLock();
        try
        {
            var dataFile = zipArchive.GetOrCreateEntry($"{Name}.data");
            using var dataStream = dataFile!.Open();
            using var dataReader = new BinaryReader(dataStream);

            if (dataStream.Position < dataStream.Length)
                Cache = EntitySerializer.Read(dataReader);
            else
                Cache = new T()
                {
                    Id = 1
                };

            EntityExtender.ExtendEntity(Cache, DbContextObject);
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
            EntitySerializer.Write(dataWriter, Cache!);
        }
        finally
        {
            Lock.ExitReadLock();
        }
    }
}
