
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;

namespace VideoEditorD3D.Database
{
    public class BinarySerializer<T>
    {
        private static readonly Lazy<(Action<BinaryWriter, T> Writer, Func<BinaryReader, T> Reader)> CachedSerializer =
            new(() => CreateDelegates());

        private static (Action<BinaryWriter, T>, Func<BinaryReader, T>) CreateDelegates()
        {
            var type = typeof(T);
            var serializerClassName = $"{type.Name}BinarySerializer";
            var code = GenerateSerializerCode(type, serializerClassName);
            var asm = Compile(code);
            var serializerType = asm.GetType(serializerClassName)!;

            var writeMethod = serializerType.GetMethod("Write")!;
            var readMethod = serializerType.GetMethod("Read")!;

            var writer = (Action<BinaryWriter, T>)Delegate.CreateDelegate(typeof(Action<BinaryWriter, T>), writeMethod)!;
            var reader = (Func<BinaryReader, T>)Delegate.CreateDelegate(typeof(Func<BinaryReader, T>), readMethod)!;

            return (writer, reader);
        }

        private static string GenerateSerializerCode(Type type, string serializerName)
        {
            var itemClassName = type.Name;
            var fullClassName = type.FullName;

            var props = type.GetProperties()
                .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null)
                .ToArray();

            var writeCode = string.Join("\n", props.Select(p =>
                $"writer.Write(value.{p.Name});"));

            var readAssignments = string.Join("\n", props.Select(p =>
                $"{p.PropertyType.FullName} {p.Name.ToLower()} = reader.Read{GetBinaryReadMethod(p.PropertyType)}();"));

            var setProps = string.Join("\n", props.Select(p =>
                $"{p.Name} = {p.Name.ToLower()},"));

            return $@"
                using System;
                using System.IO;

                public static class {serializerName}
                {{
                    public static void Write(BinaryWriter writer, {fullClassName} value)
                    {{
                        {writeCode}
                    }}

                    public static {fullClassName} Read(BinaryReader reader)
                    {{
                        {readAssignments}
                        return new {fullClassName}
                        {{
                            {setProps}
                        }};
                    }}
                }}
            ";
        }

        private static string GetBinaryReadMethod(Type type)
        {
            if (type == typeof(int)) return "Int32";
            if (type == typeof(long)) return "Int64";
            if (type == typeof(string)) return "String";
            if (type == typeof(bool)) return "Boolean";
            if (type == typeof(float)) return "Single";
            if (type == typeof(double)) return "Double";
            throw new NotSupportedException($"Unsupported type: {type.Name}");
        }

        private static Assembly Compile(string code)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var refs = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location));

            var compilation = CSharpCompilation.Create(
                assemblyName: "GeneratedSerializers_" + Guid.NewGuid(),
                syntaxTrees: new[] { syntaxTree },
                references: refs,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using var ms = new MemoryStream();
            var result = compilation.Emit(ms);

            if (!result.Success)
            {
                var errors = string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
                throw new Exception($"Compile error:\n{errors}");
            }

            ms.Position = 0;
            return Assembly.Load(ms.ToArray());
        }

        public void Write(BinaryWriter bw, T item) => CachedSerializer.Value.Writer(bw, item);
        public T Read(BinaryReader br) => CachedSerializer.Value.Reader(br);
    }

    //public class BinarySerializer<T> where T : IEntity
    //{
    //    private Action<BinaryWriter, T> WriterDelegate;
    //    private Func<BinaryReader, T> ReadDelegate;

    //    public BinarySerializer()
    //    {
    //        var type = typeof(T);
    //        var code = GenerateSerializerCode(type);
    //        var asm = Compile(code);
    //        var serializerType = asm.GetType($"{type.Name}BinarySerializer")!;

    //        var writeMethod = serializerType.GetMethod("Write")!;
    //        var readMethod = serializerType.GetMethod("Read")!;

    //        WriterDelegate = (Action<BinaryWriter, T>)Delegate.CreateDelegate(
    //            typeof(Action<BinaryWriter, T>), writeMethod)!;

    //        ReadDelegate = (Func<BinaryReader, T>)Delegate.CreateDelegate(
    //            typeof(Func<BinaryReader, T>), readMethod)!;
    //    }
    //    private static string GenerateSerializerCode(Type type)
    //    {
    //        var className = type.Name;
    //        var fullClassName = type.Namespace + "." + className;
    //        var props = type.GetProperties();

    //        var writeCode = string.Join("\n", props.Select(p =>
    //        {
    //            return $"writer.Write(value.{p.Name});";
    //        }));

    //        var readAssignments = string.Join("\n", props.Select(p =>
    //        {
    //            return $"{p.PropertyType.Name} {p.Name.ToLower()} = reader.Read{GetBinaryReadMethod(p.PropertyType)}();";
    //        }));

    //        var setProps = string.Join("\n", props.Select(p =>
    //        {
    //            return $"{p.Name} = {p.Name.ToLower()},";
    //        }));

    //        return $@"
    //            using System;
    //            using System.IO;

    //            public static class {className}BinarySerializer
    //            {{
    //                public static void Write(BinaryWriter writer, {fullClassName} value)
    //                {{
    //                    {writeCode}
    //                }}

    //                public static {fullClassName} Read(BinaryReader reader)
    //                {{
    //                    {readAssignments}
    //                    return new {fullClassName}
    //                    {{
    //                        {setProps}
    //                    }};
    //                }}
    //            }}
    //            ";
    //    }
    //    private static string GetBinaryReadMethod(Type type)
    //    {
    //        if (type == typeof(int)) return "Int32";
    //        if (type == typeof(long)) return "Int64";
    //        if (type == typeof(string)) return "String";
    //        if (type == typeof(bool)) return "Boolean";
    //        if (type == typeof(float)) return "Single";
    //        if (type == typeof(double)) return "Double";
    //        // Add more as needed
    //        throw new NotSupportedException($"Unsupported type: {type.Name}");
    //    }
    //    private static Assembly Compile(string code)
    //    {
    //        var syntaxTree = CSharpSyntaxTree.ParseText(code);
    //        var refs = AppDomain.CurrentDomain.GetAssemblies()
    //            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
    //            .Select(a => MetadataReference.CreateFromFile(a.Location))
    //            .Cast<MetadataReference>();

    //        var compilation = CSharpCompilation.Create(
    //            "GeneratedSerializers",
    //            new[] { syntaxTree },
    //            refs,
    //            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
    //        );

    //        using var ms = new MemoryStream();
    //        var result = compilation.Emit(ms);
    //        if (!result.Success)
    //        {
    //            var errors = string.Join("\n", result.Diagnostics.Where(d => d.Severity == DiagnosticSeverity.Error));
    //            throw new Exception($"Compile error:\n{errors}");
    //        }

    //        ms.Seek(0, SeekOrigin.Begin);
    //        return Assembly.Load(ms.ToArray());
    //    }
    //    public void Write(BinaryWriter bw, T item)
    //    {
    //        WriterDelegate(bw, item);
    //    }
    //    public T Read(BinaryReader bw)
    //    {
    //        return ReadDelegate(bw);
    //    }

    //}
}
