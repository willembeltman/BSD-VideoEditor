using VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;

namespace VideoEditorD3D.Entities.ZipDatabase;

public static class EntityExtenderCollection
{
    private static readonly Dictionary<Type, object> Serializers = new();

    public static EntityExtender<T> GetEntityExtender<T>(DbContext dbContext)
    {
        var type = typeof(T);
        var serializer = Serializers.ContainsKey(type) ? Serializers[type] : null;
        if (serializer == null)
        {
            var newSerializer = new EntityExtender<T>(dbContext);
            Serializers[type] = newSerializer;
            return newSerializer;
        }
        else
        {
            return (EntityExtender<T>)serializer;
        }
    }
}
