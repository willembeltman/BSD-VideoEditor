using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using VideoEditorD3D.Entities.ZipDatabase.Collections;
using VideoEditorD3D.Entities.ZipDatabase.Helpers;
using VideoEditorD3D.Loggers;

namespace VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;

public class EntityExtender<T>
{
    private Action<T, DbContext> ExtendEntityDelegate;

    public readonly string Code;

    internal EntityExtender(DbContext dbContext, ILogger logger)
    {
        var type = typeof(T);
        var className = $"{type.Name}EntityExtender";
        var methodName = "ExtendEntity";

        Code = GenerateSerializerCode(type, className, methodName, dbContext);
        logger.WriteLine($"Generated {className}:\r\n{Code}");
        var asm = Compile(Code);
        var serializerType = asm.GetType(className)!;
        var createProxyMethod = serializerType.GetMethod(methodName)!;

        ExtendEntityDelegate = (Action<T, DbContext>)Delegate.CreateDelegate(
            typeof(Action<T, DbContext>), createProxyMethod)!;
    }

    private string GenerateSerializerCode(Type type, string proxyName, string methodName, DbContext dbContext)
    {
        var className = type.Name;
        var fullClassName = type.FullName;

        var lazyCode = string.Empty;

        var foreignEntityCollectionType = typeof(ForeignEntityCollection<,>);
        var foreignEntityCollectionFullName = foreignEntityCollectionType.FullName!.Split('`').First();

        var entitySerializerType = typeof(EntitySerializer<>);
        var entitySerializerFullName = entitySerializerType.FullName!.Split('`').First();

        var dbContextType = typeof(DbContext);
        var dbContextTypeFullName = dbContextType.FullName;

        var binarySerializerCollectionType = typeof(EntitySerializerCollection);
        var binarySerializerCollectionTypeFullName = binarySerializerCollectionType.FullName;

        var applicationDbContextType = dbContext.GetType();
        var applicationDbContextTypeFullName = applicationDbContextType.FullName;

        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!ReflectionHelper.HasPublicGetter(prop)) continue;
            if (!ReflectionHelper.IsVirtual(prop)) continue;
            if (!ReflectionHelper.HasForeignKeyAttribute(prop)) continue;

            var propertyName = prop.Name;
            var foreignKeyName = ReflectionHelper.GetForeignKeyAttributeName(prop);

            if (ReflectionHelper.IsICollection(prop))
            {
                var foreignType = ReflectionHelper.GetICollectionType(prop);

                var foreignPropertyOnApplicationDbContext = applicationDbContextType.GetProperties()
                    .Where(a => ReflectionHelper.IsDbSet(a))
                    .FirstOrDefault(a => ReflectionHelper.GetDbSetType(a) == foreignType);
                if (foreignPropertyOnApplicationDbContext == null) continue;

                var foreignPropertyOnApplicationDbContextName = foreignPropertyOnApplicationDbContext.Name; 

                lazyCode += $@"
        item.{propertyName} = new {foreignEntityCollectionFullName}<{foreignType.FullName}, {fullClassName}>(
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

                var lazyPropertyOnApplicationDbContextName = lazyPropertyOnApplicationDbContext.Name; 

                lazyCode += @$"
        item.{propertyName} = new Lazy<{lazyType.FullName}?>(
            () => db.{lazyPropertyOnApplicationDbContextName}.FirstOrDefault(t => t.Id == item.{foreignKeyName}));";
            }
        }

        return $@"
using System;
using System.Linq;
using System.Collections.Generic;

public static class {proxyName}
{{
    public static void {methodName}({fullClassName} item, {dbContextTypeFullName} objDb)
    {{
        var db = objDb as {applicationDbContextTypeFullName};
{lazyCode}
    }}
}}";
    }

    public static string? GetCSharpTypeName(Type type)
    {
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
            "GeneratedEntityExtenders",
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

    public void ExtendEntity(T entity, DbContext dbContext)
    {
        ExtendEntityDelegate(entity, dbContext);
    }
}
