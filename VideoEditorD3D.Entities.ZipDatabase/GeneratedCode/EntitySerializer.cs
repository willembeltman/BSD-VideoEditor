using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using VideoEditorD3D.Entities.ZipDatabase.Helpers;

namespace VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;

public class EntitySerializer<T>
{
    private Action<BinaryWriter, T> WriteDelegate;
    private Func<BinaryReader, T> ReadDelegate;
    public readonly string Code;

    internal EntitySerializer()
    {
        var type = typeof(T);
        var className = $"{type.Name}EntitySerializer";
        var readMethodName = "EntitySerializerRead";
        var writeMethodName = "EntitySerializerWrite";
        Code = GenerateSerializerCode(type, className, readMethodName, writeMethodName);
        var asm = Compile(Code);
        var serializerType = asm.GetType(className)!;
        var readMethod = serializerType.GetMethod(readMethodName)!;
        var writeMethod = serializerType.GetMethod(writeMethodName)!;

        ReadDelegate = (Func<BinaryReader, T>)Delegate.CreateDelegate(
            typeof(Func<BinaryReader, T>), readMethod)!;

        WriteDelegate = (Action<BinaryWriter, T>)Delegate.CreateDelegate(
            typeof(Action<BinaryWriter, T>), writeMethod)!;
    }

    private string GenerateSerializerCode(Type type, string serializerName, string readMethodName, string writeMethodName)
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

        var binarySerializerCollectionType = typeof(EntitySerializerCollection);
        var binarySerializerCollectionTypeFullName = binarySerializerCollectionType.FullName;
        var method = binarySerializerCollectionType.GetMethods().First().Name;

        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!ReflectionHelper.HasPublicGetter(prop)) continue;
            if (!ReflectionHelper.HasPublicSetter(prop)) continue;
            //if (ReflectionHelper.IsVirtual(prop)) continue;
            if (ReflectionHelper.HasNotMappedAttribute(prop)) continue;
            if (ReflectionHelper.HasForeignKeyAttribute(prop)) continue;

            var propertyName = prop.Name;

            var readMethod = GetBinaryReadMethod(prop.PropertyType);
            if (readMethod != null)
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
            else
            {
                writeCode += @$"

                        var {propertyName}Serializer = {binarySerializerCollectionTypeFullName}.{method}<{prop.PropertyType.FullName}>();
                        {propertyName}Serializer.Write(writer, value.{propertyName});";

                readCode += @$"

                        var {propertyName}Serializer = {binarySerializerCollectionTypeFullName}.{method}<{prop.PropertyType.FullName}>();
                        var {propertyName} = {propertyName}Serializer.Read(reader);";

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
                    public static void {writeMethodName}(BinaryWriter writer, {fullClassName} value)
                    {{{writeCode}
                    }}

                    public static {fullClassName} {readMethodName}(BinaryReader reader)
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

    public void Write(BinaryWriter bw, T item)
    {
        WriteDelegate(bw, item);
    }
    public T Read(BinaryReader bw)
    {
        return ReadDelegate(bw);
    }
}
