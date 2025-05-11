using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace VideoEditorD3D.Entities.ZipDatabase.Helpers
{
    public static class ReflectionHelper
    {
        // Controleert of de eigenschap een foreign key attribuut heeft
        public static bool HasForeignKeyProperty(PropertyInfo prop)
        {
            return prop.GetCustomAttribute<ForeignKeyAttribute>() != null;
        }

        // Haalt de naam op van de foreign key zoals aangegeven in het [ForeignKey("...")] attribuut
        public static string? GetForeignKeyName(PropertyInfo prop)
        {
            var attr = prop.GetCustomAttribute<ForeignKeyAttribute>();
            return attr?.Name;
        }

        // Controleert of de eigenschap een ICollection<T> is (gebruikelijk voor navigatiecollecties)
        public static bool IsICollection(PropertyInfo prop)
        {
            if (prop.PropertyType == typeof(string)) return false;
            if (!prop.PropertyType.IsGenericType) return false;

            var typeDef = prop.PropertyType.GetGenericTypeDefinition();
            return typeof(ICollection<>).IsAssignableFrom(typeDef) ||
                   prop.PropertyType.GetInterfaces()
                       .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
        }

        // Controleert of de property publiek toegankelijk is (ten minste met een getter)
        public static bool IsPublic(PropertyInfo prop)
        {
            var getter = prop.GetGetMethod(false);
            return getter != null;
        }

        // Controleert of de property een publieke of private setter heeft
        public static bool HasSetter(PropertyInfo prop)
        {
            return prop.GetSetMethod(false) != null;
        }
        public static bool IsLazy(PropertyInfo prop)
        {
            var type = prop.PropertyType;

            return type.IsGenericType &&
                   type.GetGenericTypeDefinition() == typeof(Lazy<>);
        }


        public static Type GetICollectionType(PropertyInfo prop)
        {
            return prop.PropertyType.GenericTypeArguments[0];
        }
        public static Type GetLazyType(PropertyInfo prop)
        {
            return prop.PropertyType.GenericTypeArguments[0];
        }

        public static bool IsNulleble(PropertyInfo prop)
        {
            var type = prop.PropertyType;
            if (type.IsValueType)
            {
                return Nullable.GetUnderlyingType(type) != null;
            }
            else
            {
                return true; // Referentietypen zijn altijd nullable
            }
        }

        public static bool IsVirtual(PropertyInfo prop)
        {
            var method = prop.GetGetMethod(true);
            if (method == null)
                return false;
            return method.IsVirtual && !method.IsFinal;
        }
        public static bool IsDbSet(PropertyInfo prop)
        {
            if (!prop.PropertyType.IsGenericType)
                return false;

            var typeDef = prop.PropertyType.GetGenericTypeDefinition();
            return typeDef == typeof(DbSet<>);
        }

        public static Type GetDbSetType(PropertyInfo a)
        {
            var res = a.PropertyType.GenericTypeArguments[0];
            return res;
        }

        public static Type GetPropertyType(PropertyInfo prop)
        {
            return prop.PropertyType;
        }
    }

}
