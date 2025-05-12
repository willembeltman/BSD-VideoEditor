using VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;

namespace VideoEditorD3D.Entities.ZipDatabase.Collections;

public static class EntitySerializerCollection
{
    private static readonly Dictionary<Type, object> EntitySerializers = new();

    public static EntitySerializer<T> GetOrCreate<T>()
    {
        var type = typeof(T);
        var serializer = EntitySerializers.ContainsKey(type) ? EntitySerializers[type] : null;
        if (serializer == null)
        {
            var newSerializer = new EntitySerializer<T>();
            EntitySerializers[type] = newSerializer;
            return newSerializer;
        }
        else
        {
            return (EntitySerializer<T>)serializer;
        }
    }
}
