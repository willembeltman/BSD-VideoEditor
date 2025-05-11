using VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;
using VideoEditorD3D.Loggers;

namespace VideoEditorD3D.Entities.ZipDatabase;

public static class EntityExtenderCollection
{
    private static readonly Dictionary<Type, object> Serializers = new();

    public static EntityExtender<T> GetEntityExtender<T>(DbContext dbContext, ILogger logger)
    {
        var type = typeof(T);
        var serializer = Serializers.ContainsKey(type) ? Serializers[type] : null;
        if (serializer == null)
        {
            var newSerializer = new EntityExtender<T>(dbContext, logger);
            Serializers[type] = newSerializer;
            return newSerializer;
        }
        else
        {
            return (EntityExtender<T>)serializer;
        }
    }
}
