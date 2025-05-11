using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Reflection;
using VideoEditorD3D.Entities.ZipDatabase.Helpers;

namespace VideoEditorD3D.Entities.ZipDatabase;

public class EntityProxyCreater<T>
{
    private Func<T, DbContext, T> CreateProxyDelegate;

    internal EntityProxyCreater(DbContext dbContext)
    {
        var type = typeof(T);
        var proxyName = $"{type.Name}EntityProxy";
        var code = GenerateSerializerCode(type, proxyName, dbContext);
        var asm = Compile(code);
        var serializerType = asm.GetType(proxyName)!;

        var createProxyMethod = serializerType.GetMethod("CreateProxy")!;

        CreateProxyDelegate = (Func<T, DbContext, T>)Delegate.CreateDelegate(
            typeof(Func<T, DbContext, T>), createProxyMethod)!;
    }

    private string GenerateSerializerCode(Type type, string proxyName, DbContext dbContext)
    {
        var className = type.Name;
        var fullClassName = type.FullName;

        var propertiesCode = string.Empty;

        var entityDbCollectionType = typeof(EntityProxyForeignCollection<,>);
        var entityDbCollectionTypeFullName = entityDbCollectionType.FullName!.Split('`').First();

        var binarySerializerType = typeof(BinarySerializer<>);
        var binarySerializerTypeFullName = binarySerializerType.FullName!.Split('`').First();

        var dbContextType = typeof(DbContext);
        var dbContextTypeFullName = dbContextType.FullName;

        var binarySerializerCollectionType = typeof(BinarySerializerCollection);
        var binarySerializerCollectionTypeFullName = binarySerializerCollectionType.FullName;

        var applicationDbContextType = dbContext.ParentType;
        var applicationDbContextTypeFullName = dbContext.ParentType.FullName;

        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!ReflectionHelper.IsPublic(prop)) continue;

            var propertyName = prop.Name;
            var isNulleble = ReflectionHelper.IsNulleble(prop);

            if (ReflectionHelper.IsVirtual(prop))
            {
                if (!ReflectionHelper.HasSetter(prop)) continue;

                if (ReflectionHelper.HasForeignKeyProperty(prop))
                {
                    var foreignKeyName = ReflectionHelper.GetForeignKeyName(prop);

                    if (ReflectionHelper.IsICollection(prop))
                    {
                        var foreignType = ReflectionHelper.GetICollectionType(prop);
                        var foreignPropertyOnApplicationDbContext = applicationDbContextType.GetProperties()
                            .Where(a => ReflectionHelper.IsDbSet(a))
                            .FirstOrDefault(a => ReflectionHelper.GetDbSetType(a) == foreignType);
                        if (foreignPropertyOnApplicationDbContext == null) continue;
                        var foreignPropertyOnApplicationDbContextName = foreignPropertyOnApplicationDbContext.Name; // "Files"

                        propertiesCode += $@"

                            private ICollection<{foreignType.FullName}>? _{propertyName};
                            public override ICollection<{foreignType.FullName}> {propertyName}
                            {{
                                get
                                {{
                                    if (_{propertyName} == null)
                                    {{
                                        _{propertyName} = new {entityDbCollectionTypeFullName}<{foreignType.FullName}, {fullClassName}>(Db.{foreignPropertyOnApplicationDbContextName}, Item, (a, b) => a.{foreignKeyName} == b.Id, (a, b) => a.{foreignKeyName} = b.Id);
                                    }}
                                    return _{propertyName};
                                }}
                                set
                                {{
                                    _{propertyName} = value;
                                }}
                            }}";
                    }
                    else
                    {
                        var lazyType = ReflectionHelper.GetPropertyType(prop);
                        var lazyPropertyOnApplicationDbContext = applicationDbContextType.GetProperties()
                            .Where(a => ReflectionHelper.IsDbSet(a))
                            .FirstOrDefault(a => ReflectionHelper.GetDbSetType(a) == lazyType);
                        if (lazyPropertyOnApplicationDbContext == null) continue;
                        var lazyPropertyOnApplicationDbContextName = lazyPropertyOnApplicationDbContext.Name;

                        propertiesCode += $@"

                            private {lazyType.FullName}? _{propertyName} {{ get; set; }}
                            public override {lazyType.FullName}? {propertyName}
                            {{
                                get
                                {{
                                    if (_{propertyName} == null)
                                    {{
                                        _{propertyName} = Db.{lazyPropertyOnApplicationDbContextName}.FirstOrDefault(a => a.Id == {foreignKeyName});
                                    }}
                                    return _{propertyName};
                                }}
                                set
                                {{
                                    _{propertyName} = value;
                                }}
                            }}";
                    }
                }
            }
            else
            {
                var propertyType = ReflectionHelper.GetPropertyType(prop);
                var propertyTypeName = GetCSharpTypeName(propertyType);

                //if (ReflectionHelper.HasForeignKeyProperty(prop))
                //{
                //    var foreignKeyName = ReflectionHelper.GetForeignKeyName(prop);

                //    if (ReflectionHelper.IsICollection(prop))
                //    {
                //        var foreignType = ReflectionHelper.GetICollectionType(prop);
                //        var foreignPropertyOnApplicationDbContext = applicationDbContextType.GetProperties()
                //            .Where(a => ReflectionHelper.IsDbSet(a))
                //            .FirstOrDefault(a => ReflectionHelper.GetDbSetType(a) == foreignType);
                //        if (foreignPropertyOnApplicationDbContext == null) continue;
                //        var foreignPropertyOnApplicationDbContextName = foreignPropertyOnApplicationDbContext.Name; 

                //        propertiesCode += $@"

                //            private ICollection<{foreignType.FullName}>? _{propertyName};
                //            public new ICollection<{foreignType.FullName}> {propertyName}
                //            {{
                //                get
                //                {{
                //                    if (_{propertyName} == null)
                //                    {{
                //                        _{propertyName} = new {entityDbCollectionTypeFullName}<{foreignType.FullName}, {fullClassName}>(Db.{foreignPropertyOnApplicationDbContextName}, Item, (a, b) => a.{foreignKeyName} == b.Id, (a, b) => a.{foreignKeyName} = b.Id);
                //                    }}
                //                    return _{propertyName};
                //                }}
                //                set
                //                {{
                //                    _{propertyName} = value;
                //                }}
                //            }}";
                //    }
                //    else if (ReflectionHelper.IsLazy(prop))
                //    {
                //        var lazyType = ReflectionHelper.GetLazyType(prop);
                //        var lazyPropertyOnApplicationDbContext = applicationDbContextType.GetProperties()
                //            .Where(a => ReflectionHelper.IsDbSet(a))
                //            .FirstOrDefault(a => ReflectionHelper.GetDbSetType(a) == lazyType);
                //        if (lazyPropertyOnApplicationDbContext == null) continue;
                //        var lazyPropertyOnApplicationDbContextName = lazyPropertyOnApplicationDbContext.Name; // "Files"

                //        propertiesCode += @$"

                //            public Lazy<{lazyType.FullName}>? _{propertyName};
                //            public new Lazy<{lazyType.FullName}>? {propertyName}
                //            {{
                //                get
                //                {{
                //                    if (_{propertyName} == null)
                //                    {{
                //                        _{propertyName} = new Lazy<{lazyType.FullName}>(() => Db.{lazyPropertyOnApplicationDbContextName}.FirstOrDefault(t => t.Id == Item.{foreignKeyName}));
                //                    }}
                //                    return _{propertyName};
                //                }}
                //                set
                //                {{
                //                    _{propertyName} = value;
                //                }}
                //            }}";
                //    }
                //}
                //else
                //{
                if (ReflectionHelper.HasSetter(prop))
                {
                    propertiesCode += $@"

                            public new {propertyTypeName} {propertyName} {{ get => Item.{propertyName}; set => Item.{propertyName} = value; }}";
                }
                else
                {
                    propertiesCode += $@"

                            public new {propertyTypeName} {propertyName} => Item.{propertyName};";

                }
                //}
            }
        }

        return $@"
                        using System;
                        using System.IO;
                        using System.Linq;
                        using System.Collections.Generic;

                        public class {proxyName} : {fullClassName}
                        {{
                            private readonly {fullClassName} Item;
                            private readonly {applicationDbContextTypeFullName} Db;

                            public {proxyName}({fullClassName} item, {dbContextTypeFullName} db)
                            {{
                                Item = item;
                                Db = db as {applicationDbContextTypeFullName};
                            }}{propertiesCode}

                            public static {fullClassName} CreateProxy({fullClassName} item, {dbContextTypeFullName} db)
                            {{
                                return new {proxyName}(item, db);
                            }}
                        }}
";
    }

    public static string GetCSharpTypeName(Type type)
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

    public T CreateProxy(T entity, DbContext dbContext)
    {
        return CreateProxyDelegate(entity, dbContext);
    }
}
