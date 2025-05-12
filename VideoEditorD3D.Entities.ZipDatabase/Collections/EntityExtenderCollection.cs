using VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;
using VideoEditorD3D.Loggers;

namespace VideoEditorD3D.Entities.ZipDatabase.Collections;

public static class EntityExtenderCollection
{
    private static readonly Dictionary<Type, object> EntityExtenders = new();

    public static EntityExtender<T> GetOrCreate<T>(DbContext dbContext, ILogger logger)
    {
        var type = typeof(T);
        var serializer = EntityExtenders.ContainsKey(type) ? EntityExtenders[type] : null;
        if (serializer == null)
        {
            var newSerializer = new EntityExtender<T>(dbContext, logger);
            EntityExtenders[type] = newSerializer;
            return newSerializer;
        }
        else
        {
            return (EntityExtender<T>)serializer;
        }
    }
}
