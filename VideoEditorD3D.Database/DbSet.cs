using System.Collections;
using System.IO.Compression;

namespace VideoEditorD3D.Database
{
    public class DbSet<T> : ICollection<T>, IDbSet
        where T : IEntity
    {
        public DbSet(DbContext dbContext)
        {
            dbContext.AddDbSet(this);

            Name = typeof(T).Name;
            Lock = new ReaderWriterLockSlim();
            Cache = new Dictionary<long, T>();
            Serializer = new BinarySerializer<T>();

            LoadCache(dbContext.ZipArchive);
        }

        private readonly string Name;
        private readonly ReaderWriterLockSlim Lock;
        private readonly Dictionary<long, T> Cache;
        private readonly BinarySerializer<T> Serializer;

        private long LastId;


        private void LoadCache(ZipArchive zipArchive)
        {
            var idFile = zipArchive.GetOrCreateEntry($"{Name}.id");
            using var idStream = idFile!.Open();
            using var idReader = new BinaryReader(idStream);

            var indexFile = zipArchive.GetOrCreateEntry($"{Name}.index");
            using var indexStream = indexFile!.Open();
            using var indexReader = new BinaryReader(indexStream);

            var dataFile = zipArchive.GetOrCreateEntry($"{Name}.data");
            using var dataStream = dataFile!.Open();
            using var dataReader = new BinaryReader(dataStream);

            if (idStream.Position < idStream.Length)
                LastId = idReader.ReadInt64();

            while (indexStream!.Position < indexStream.Length)
            {
                var indexPosition = indexStream.Position;
                var dataPosition = indexReader!.ReadInt64();
                if (dataPosition >= 0)
                {
                    dataStream.Position = dataPosition;
                    var item = Serializer.Read(dataReader!);
                    Cache[item.Id] = item;
                }
            }
        }
        public void WriteCache(ZipArchive zipArchive)
        {
            var idFile = zipArchive.GetOrCreateEntry($"{Name}.id");
            using var idStream = idFile!.Open();
            using var idWriter = new BinaryWriter(idStream);

            var indexFile = zipArchive.GetOrCreateEntry($"{Name}.index");
            using var indexStream = indexFile!.Open();
            using var indexWriter = new BinaryWriter(indexStream);

            var dataFile = zipArchive.GetOrCreateEntry($"{Name}.data");
            using var dataStream = dataFile!.Open();
            using var dataWriter = new BinaryWriter(dataStream);

            idWriter.Write(LastId);
            foreach (var item in Cache.Values)
            {
                indexWriter.Write(dataStream.Position);
                Serializer.Write(dataWriter, item);
            }
        }

        #region ICollection
        public int Count
        {
            get
            {
                Lock.EnterReadLock();
                try { return Cache.Count; }
                finally { Lock.ExitReadLock(); }
            }
        }
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Lock.EnterWriteLock();
            try
            {
                item.Id = ++LastId;
                Cache[item.Id] = item;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }
        public bool Remove(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            Lock.EnterWriteLock();
            try
            {
                if (!Cache.TryGetValue(item.Id, out var entry)) return false;
                Cache.Remove(item.Id);
                return true;
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }
        public void Clear()
        {
            Lock.EnterWriteLock();
            try
            {
                Cache.Clear();
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }
        public bool Contains(T item)
        {
            if (item == null) return false;

            Lock.EnterReadLock();
            try
            {
                return Cache.ContainsKey(item.Id);
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            Lock.EnterReadLock();
            try
            {
                foreach (var item in Cache.Values)
                {
                    if (arrayIndex >= array.Length) throw new ArgumentException("Target array too small");
                    array[arrayIndex++] = item;
                }
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }
        public IEnumerator<T> GetEnumerator()
        {
            Lock.EnterReadLock();
            try
            {
                return Cache.Values.GetEnumerator(); // safe snapshot
            }
            finally
            {
                Lock.ExitReadLock();
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
