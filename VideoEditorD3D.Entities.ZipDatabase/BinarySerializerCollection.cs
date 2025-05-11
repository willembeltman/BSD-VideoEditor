namespace VideoEditorD3D.Entities.ZipDatabase;

public static class BinarySerializerCollection
{
    private static readonly Dictionary<Type, object> Serializers = new();

    public static BinarySerializer<T> GetSerializer<T>(DbContext dbContext)
    {
        var type = typeof(T);
        var serializer = Serializers.ContainsKey(type) ? Serializers[type] : null;
        if (serializer == null)
        {
            var newSerializer = new BinarySerializer<T>(dbContext);
            Serializers[type] = newSerializer;
            return newSerializer;
        }
        else
        {
            return (BinarySerializer<T>)serializer;
        }
    }
}
