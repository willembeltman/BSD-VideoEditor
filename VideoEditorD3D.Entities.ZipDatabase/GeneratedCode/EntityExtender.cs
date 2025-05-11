using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using System.Text.Json.Serialization;
using VideoEditorD3D.Entities.ZipDatabase.Helpers;

namespace VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;

public class EntityExtender<T>
{
    private Action<T, object> ExtendEntityDelegate;

    public readonly string Code;

    internal EntityExtender(DbContext dbContext)
    {
        var type = typeof(T);
        var className = $"{type.Name}EntityExtender";
        var methodName = "ExtendEntity";

        Code = GenerateSerializerCode(type, className, methodName, dbContext);
        var asm = Compile(Code);
        var serializerType = asm.GetType(className)!;
        var createProxyMethod = serializerType.GetMethod(methodName)!;

        ExtendEntityDelegate = (Action<T, object>)Delegate.CreateDelegate(
            typeof(Action<T, object>), createProxyMethod)!;
    }

    private string GenerateSerializerCode(Type type, string proxyName, string methodName, DbContext dbContext)
    {
        var className = type.Name;
        var fullClassName = type.FullName;

        var lazyCode = string.Empty;

        var entityDbCollectionType = typeof(ForeignEntityCollection<,>);
        var entityDbCollectionTypeFullName = entityDbCollectionType.FullName!.Split('`').First();

        var binarySerializerType = typeof(EntitySerializer<>);
        var binarySerializerTypeFullName = binarySerializerType.FullName!.Split('`').First();

        var dbContextType = typeof(DbContext);
        var dbContextTypeFullName = dbContextType.FullName;

        var binarySerializerCollectionType = typeof(EntitySerializerCollection);
        var binarySerializerCollectionTypeFullName = binarySerializerCollectionType.FullName;

        var applicationDbContextType = dbContext.ParentType;
        var applicationDbContextTypeFullName = dbContext.ParentType.FullName;

        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!ReflectionHelper.HasPublicGetter(prop)) continue;

            var propertyName = prop.Name;
            var isNulleble = ReflectionHelper.IsNulleble(prop);

            if (!ReflectionHelper.HasForeignKeyAttribute(prop)) continue;
            //if (!ReflectionHelper.(prop)) continue;

            var foreignKeyName = ReflectionHelper.GetForeignKeyAttributeName(prop);

            if (ReflectionHelper.IsICollection(prop))
            {
                var foreignType = ReflectionHelper.GetICollectionType(prop);

                var foreignPropertyOnApplicationDbContext = applicationDbContextType.GetProperties()
                    .Where(a => ReflectionHelper.IsDbSet(a))
                    .FirstOrDefault(a => ReflectionHelper.GetDbSetType(a) == foreignType);
                if (foreignPropertyOnApplicationDbContext == null) continue;

                var foreignPropertyOnApplicationDbContextName = foreignPropertyOnApplicationDbContext.Name; // "Files"

                

                lazyCode += $@"
                        //System.Windows.Forms.MessageBox.Show(""Hello, World! \r\n"" + JsonConvert.SerializeObject((({applicationDbContextTypeFullName})db)));
                        item.{propertyName} = new {entityDbCollectionTypeFullName}<{foreignType.FullName}, {fullClassName}>(
                            db.{foreignPropertyOnApplicationDbContextName},
                            item,
                            (foreign, primary) => foreign.{foreignKeyName} == primary.Id,
                            (foreign, primary) => foreign.{foreignKeyName} = primary.Id);";
            }
            else if (ReflectionHelper.IsLazy(prop))
            {
                var lazyType = ReflectionHelper.GetLazyType(prop);

                var lazyPropertyOnApplicationDbContext = applicationDbContextType.GetProperties()
                    .Where(a => ReflectionHelper.IsDbSet(a))
                    .FirstOrDefault(a => ReflectionHelper.GetDbSetType(a) == lazyType);
                if (lazyPropertyOnApplicationDbContext == null) continue;

                var lazyPropertyOnApplicationDbContextName = lazyPropertyOnApplicationDbContext.Name; // "Files"

                lazyCode += @$"
                        item.{propertyName} = new Lazy<{lazyType.FullName}?>(
                            () => db.{lazyPropertyOnApplicationDbContextName}.FirstOrDefault(t => t.Id == item.{foreignKeyName}));";
            }
        }

        return $@"
                using System;
                using System.Linq;
                using System.Collections.Generic;
                using Newtonsoft.Json;

                public static class {proxyName}
                {{
                    public static void {methodName}({fullClassName} item, object objDb)
                    {{
                        var db = objDb as {applicationDbContextTypeFullName};
{lazyCode}
                    }}
                }}
";
    }

    public static string? GetCSharpTypeName(Type type)
    {
        //System.Diagnostics.Debug.WriteLine("ADSGSDG !!!!!!!!!!!!!!!!!");
        if (type == typeof(int)) return "int";
        if (type == typeof(long)) return "long";
        if (type == typeof(short)) return "short";
        if (type == typeof(byte)) return "byte";
        if (type == typeof(bool)) return "bool";
        if (type == typeof(char)) return "char";
        if (type == typeof(string)) return "string";
        if (type == typeof(object)) return "object";
        if (type == typeof(float)) return "float";
        if (type == typeof(double)) return "double";
        if (type == typeof(decimal)) return "decimal";
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            return GetCSharpTypeName(Nullable.GetUnderlyingType(type)!) + "?";

        return type.FullName; // fallback, e.g. for classes
    }

    private Assembly Compile(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var refs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create(
            "GeneratedExtenders",
            new[] { syntaxTree },
            refs,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        );

        using var ms = new MemoryStream();
        var result = compilation.Emit(ms);
        if (!result.Success)
        {
            var errors = string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
            throw new Exception($"Compile error:\n{errors}");
        }

        ms.Seek(0, SeekOrigin.Begin);
        return Assembly.Load(ms.ToArray());
    }

    public void ExtendEntity(T entity, object dbContext)
    {
        ExtendEntityDelegate(entity, dbContext);
    }
}
