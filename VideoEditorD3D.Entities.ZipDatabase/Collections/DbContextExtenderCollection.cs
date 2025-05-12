using VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;
using VideoEditorD3D.Loggers;

namespace VideoEditorD3D.Entities.ZipDatabase.Collections;

public static class DbContextExtenderCollection
{
    private static readonly Dictionary<Type, object> DbContextExtenders = new();

    public static DbContextExtender GetOrCreate(DbContext dbContext, ILogger logger)
    {
        var type = dbContext.GetType();
        var serializer = DbContextExtenders.ContainsKey(type) ? DbContextExtenders[type] : null;
        if (serializer == null)
        {
            var newSerializer = new DbContextExtender(dbContext, logger);
            DbContextExtenders[type] = newSerializer;
            return newSerializer;
        }
        else
        {
            return (DbContextExtender)serializer;
        }
    }
}