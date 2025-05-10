using System.IO.Compression;
using VideoEditorD3D.Entities.ZipDatabase.Interfaces;

namespace VideoEditorD3D.Entities.ZipDatabase;

public class DbContext : IDisposable
{
    public DbContext(string fullName)
    {
        FullName = fullName;
        ZipStream = File.Open(FullName!, FileMode.OpenOrCreate);
        ZipArchive = new ZipArchive(ZipStream, ZipArchiveMode.Update);
        // Hou de zip open, zodat hij gelocked is
        DbSets = new List<IDbSet>();
        DbObjects = new List<IDbObject>();
    }

    public string FullName { get; }
    internal Stream ZipStream { get; private set; }
    internal ZipArchive ZipArchive { get; private set; }

    private List<IDbSet> DbSets;
    private List<IDbObject> DbObjects;

    internal void AddDbSet(IDbSet dbSet)
    {
        DbSets.Add(dbSet);
    }
    internal void AddDbObject(IDbObject dbObject)
    {
        DbObjects.Add(dbObject);
    }
    public void SaveChanges()
    {
        ZipArchive.Dispose();
        ZipStream.Dispose();

        if (File.Exists(FullName))
            File.Delete(FullName);

        ZipStream = File.Open(FullName!, FileMode.OpenOrCreate);
        ZipArchive = new ZipArchive(ZipStream, ZipArchiveMode.Update);

        foreach (var dbSet in DbSets)
            dbSet.WriteCache(ZipArchive);

        foreach (var dbObject in DbObjects)
            dbObject.WriteCache(ZipArchive);
    }
    public void Dispose()
    {
        ZipArchive.Dispose();
        ZipStream.Dispose();
        GC.SuppressFinalize(this);
    }
}
