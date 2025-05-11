using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using VideoEditorD3D.Entities.ZipDatabase.Helpers;

namespace VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;

public class EntitySerializer<T>
{
    private Action<BinaryWriter, T, DbContext> WriteDelegate;
    private Func<BinaryReader, DbContext, T> ReadDelegate;
    private readonly string code;

    internal EntitySerializer(DbContext dbContext)
    {
        var type = typeof(T);
        var className = $"{type.Name}EntitySerializer";
        var readMethodName = "EntitySerializerRead";
        var writeMethodName = "EntitySerializerWrite";
        code = GenerateSerializerCode(type, className, readMethodName, writeMethodName, dbContext);
        var asm = Compile(code);
        var serializerType = asm.GetType(className)!;
        var readMethod = serializerType.GetMethod(readMethodName)!;
        var writeMethod = serializerType.GetMethod(writeMethodName)!;

        ReadDelegate = (Func<BinaryReader, DbContext, T>)Delegate.CreateDelegate(
            typeof(Func<BinaryReader, DbContext, T>), readMethod)!;

        WriteDelegate = (Action<BinaryWriter, T, DbContext>)Delegate.CreateDelegate(
            typeof(Action<BinaryWriter, T, DbContext>), writeMethod)!;

    }

    private string GenerateSerializerCode(Type type, string serializerName, string readMethodName, string writeMethodName, DbContext dbContext)
    {
        var className = type.Name;
        var fullClassName = type.FullName;

        var writeCode = string.Empty;
        var readCode = string.Empty;
        var newCode = string.Empty;
        //var lazyCode = string.Empty;

        var entityDbCollectionType = typeof(ForeignEntityCollection<,>);
        var entityDbCollectionTypeFullName = entityDbCollectionType.FullName!.Split('`').First();

        var binarySerializerType = typeof(EntitySerializer<>);
        var binarySerializerTypeFullName = binarySerializerType.FullName!.Split('`').First();

        var dbContextType = typeof(DbContext);
        var dbContextTypeFullName = dbContextType.FullName;

        var binarySerializerCollectionType = typeof(EntitySerializerCollection);
        var binarySerializerCollectionTypeFullName = binarySerializerCollectionType.FullName;
        var method = binarySerializerCollectionType.GetMethods().First().Name;

        var applicationDbContextType = dbContext.ParentType;
        var applicationDbContextTypeFullName = dbContext.ParentType.FullName;

        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!ReflectionHelper.IsPublic(prop)) continue;
            if (!ReflectionHelper.HasSetter(prop)) continue;
            if (ReflectionHelper.IsVirtual(prop)) continue;
            if (ReflectionHelper.HasForeignKeyProperty(prop)) continue;

            var propertyName = prop.Name;

            var readMethod = GetBinaryReadMethod(prop.PropertyType);
            if (readMethod == null)
            {
                writeCode += @$"

                        var {propertyName}Serializer = {binarySerializerCollectionTypeFullName}.{method}<{prop.PropertyType.FullName}>(db);
                        {propertyName}Serializer.Write(writer, value.{propertyName}, db);";

                readCode += @$"

                        var {propertyName}Serializer = {binarySerializerCollectionTypeFullName}.{method}<{prop.PropertyType.FullName}>(db);
                        var {propertyName} = {propertyName}Serializer.Read(reader, db);";

            }
            else
            {
                if (ReflectionHelper.IsNulleble(prop))
                {
                    writeCode += @$"

                        if (value.{propertyName} == null)
                            writer.Write(true);
                        else
                        {{
                            writer.Write(false);
                            writer.Write(value.{propertyName});
                        }}";

                    readCode += @$"

                        {prop.PropertyType.FullName} {propertyName} = null;
                        if (!reader.ReadBoolean())
                        {{
                            {propertyName} = reader.Read{readMethod}();
                        }}";
                }
                else
                {
                    writeCode += @$"
                        writer.Write(value.{propertyName});";

                    readCode += @$"
                        var {propertyName} = reader.Read{readMethod}();";
                }
            }

            newCode += @$"
                            {propertyName} = {propertyName},";
        }

        return $@"
                using System;
                using System.IO;
                using System.Linq;

                public static class {serializerName}
                {{
                    public static void {writeMethodName}(BinaryWriter writer, {fullClassName} value, {dbContextTypeFullName} db)
                    {{{writeCode}
                    }}

                    public static {fullClassName} {readMethodName}(BinaryReader reader, {dbContextTypeFullName} db)
                    {{{readCode}

                        var item = new {fullClassName}
                        {{{newCode}
                        }};

                        return item;
                    }}
                }}";
    }
    private string? GetBinaryReadMethod(Type type)
    {
        if (type == typeof(int)) return "Int32";
        if (type == typeof(long)) return "Int64";
        if (type == typeof(string)) return "String";
        if (type == typeof(bool)) return "Boolean";
        if (type == typeof(float)) return "Single";
        if (type == typeof(double)) return "Double";
        return null;
    }
    private Assembly Compile(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var refs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create(
            "GeneratedSerializers",
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

    public void Write(BinaryWriter bw, T item, DbContext dbContext)
    {
        WriteDelegate(bw, item, dbContext);
    }
    public T Read(BinaryReader bw, DbContext dbContext)
    {
        return ReadDelegate(bw, dbContext);
    }
}
