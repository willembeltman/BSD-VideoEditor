using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using VideoEditorD3D.Entities.ZipDatabase.Helpers;
using VideoEditorD3D.Loggers;

namespace VideoEditorD3D.Entities.ZipDatabase.GeneratedCode;

public class DbContextExtender
{
    private Action<DbContext, ILogger> ExtendDbContextDelegate;

    public readonly string Code;

    internal DbContextExtender(DbContext dbContext, ILogger logger)
    {
        var applicationDbContextType = dbContext.GetType(); 
        var extenderName = $"{applicationDbContextType.Name}DbContextExtender";
        var extenderMethodName = "ExtendDbContext";

        Code = GenerateSerializerCode(applicationDbContextType, extenderName, extenderMethodName, dbContext);
        logger.WriteLine($"Generated {extenderName}:\r\n{Code}");
        var asm = Compile(Code);
        var serializerType = asm.GetType(extenderName)!;
        var createProxyMethod = serializerType.GetMethod(extenderMethodName)!;

        ExtendDbContextDelegate = (Action<DbContext, ILogger>)Delegate.CreateDelegate(
            typeof(Action<DbContext, ILogger>), createProxyMethod)!;
    }

    private string GenerateSerializerCode(Type applicationDbContextType, string extenderName, string extenderMethodName, DbContext dbContext)
    {
        var applicationDbContextName = applicationDbContextType.Name;
        var applicationDbContextFullName = applicationDbContextType.FullName;

        var dbContextType = typeof(DbContext);
        var dbContextTypeFullName = dbContextType.FullName;

        var dbSetType = typeof(DbSet<>);
        var dbSetFullName = dbSetType.FullName!.Split('`').First();

        var loggerType = typeof(ILogger);
        var loggerTypeFullName = loggerType.FullName;

        var propertiesCode = string.Empty;
        var props = applicationDbContextType.GetProperties();
        foreach (var property in props)
        {
            if (!ReflectionHelper.HasPublicGetter(property)) continue;
            if (!ReflectionHelper.IsVirtual(property)) continue;
            if (!ReflectionHelper.IsDbSet(property)) continue;

            var propertyName = property.Name;

            var propertyType = ReflectionHelper.GetDbSetType(property);
            var propertyTypeName = propertyType.Name;
            var propertyTypeFullName = propertyType.FullName;

            propertiesCode += $@"
        db.{propertyName} = new {dbSetFullName}<{propertyTypeFullName}>(db, logger);";
        }

        return $@"
using System;

public static class {extenderName}
{{
    public static void {extenderMethodName}({dbContextTypeFullName} dbContext, {loggerTypeFullName} logger)
    {{
        var db = dbContext as {applicationDbContextFullName};
        if (db == null) throw new Exception(""dbContext is not of type {applicationDbContextName}"");{propertiesCode}
    }}
}}";
    }
    
    private Assembly Compile(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var refs = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Cast<MetadataReference>();

        var compilation = CSharpCompilation.Create(
            "GeneratedDbContextExtenders",
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

    public void ExtendDbContext(DbContext dbContext, ILogger logger)
    {
        ExtendDbContextDelegate(dbContext, logger);
    }
}
