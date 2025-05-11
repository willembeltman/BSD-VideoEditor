namespace VideoEditorD3D.Entities.ZipDatabase;

public static class EntityProxyCreaterCollection
{
    private static readonly Dictionary<Type, object> Serializers = new();

    public static EntityProxyCreater<T> GetEntityProxyCreator<T>(DbContext dbContext)
    {
        var type = typeof(T);
        var serializer = Serializers.ContainsKey(type) ? Serializers[type] : null;
        if (serializer == null)
        {
            var newSerializer = new EntityProxyCreater<T>(dbContext);
            Serializers[type] = newSerializer;
            return newSerializer;
        }
        else
        {
            return (EntityProxyCreater<T>)serializer;
        }
    }
}
