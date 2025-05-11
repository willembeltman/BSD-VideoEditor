using VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;

namespace VideoEditorD3D.Entities.ZipDatabase;

public static class EntitySerializerCollection
{
    private static readonly Dictionary<Type, object> Serializers = new();

    public static EntitySerializer<T> GetEntitySerializer<T>(DbContext dbContext)
    {
        var type = typeof(T);
        var serializer = Serializers.ContainsKey(type) ? Serializers[type] : null;
        if (serializer == null)
        {
            var newSerializer = new EntitySerializer<T>();
            Serializers[type] = newSerializer;
            return newSerializer;
        }
        else
        {
            return (EntitySerializer<T>)serializer;
        }
    }
}
