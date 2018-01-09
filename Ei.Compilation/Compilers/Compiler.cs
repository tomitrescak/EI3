using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Ei.Ontology;
using Ei.Logs;
using System.Collections;
using System.Diagnostics;

namespace Ei.Compilation
{
    public class Compiler
    {
        static Action<string> Write = Console.WriteLine;

        public static string Compile(string codeToCompile)
        {
            Write("Let's compile!");
            Write("Parsing the code into the SyntaxTree");

            codeToCompile = @"
        using Ei.Ontology;
        using Ei.Ontology.Actions;
        using Ei.Ontology.Transitions;
        using Ei.Runtime;
        using System.Collections.Generic;
      " + codeToCompile;

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);

            // list
            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            var neededAssemblies = new[]
            {
          "System.Runtime",
          "System.Private.CoreLib",
          "netstandard"
      };

            List<PortableExecutableReference> references = trustedAssembliesPaths
                .Where(p => neededAssemblies.Contains(Path.GetFileNameWithoutExtension(p)))
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();

            string assemblyName = Path.GetRandomFileName();

            references.Add(MetadataReference.CreateFromFile(typeof(Organisation).GetTypeInfo().Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(Log).GetTypeInfo().Assembly.Location));

            // Console.WriteLine(string.Join("\n", references.Select(s => s.Display).ToArray()));

            Write("Compiling ...");
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    Write("Compilation failed!");
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    var message = string.Join("\n", failures.Select(f => f.GetMessage()));
                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                    return message;
                }
                else
                {
                    // Write("Compilation successful! Now instantiating and executing the code ...");
                    // ms.Seek(0, SeekOrigin.Begin);

                    // Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    // var type = assembly.GetType("RoslynCompileSample.Writer");
                    // var instance = assembly.CreateInstance("RoslynCompileSample.Writer");
                    // var meth = type.GetMember("Write").First() as MethodInfo;
                    // meth.Invoke(instance, new[] { "joel" });
                }
            }
            return null;
        }
    }
}