using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Ei.Core.Ontology;
using Ei.Logs;
using System.Collections;
using System.Diagnostics;
using System.Security.Permissions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ei.Projects.Physiology;

namespace Ei.Compilation
{
    public class Compiler
    {
        static Action<string> Write = Console.WriteLine;

        public static CompilationResult Compile(string codeToCompile) {
            return Compile(codeToCompile, null, out object obj);
        }

        public class CompilationResult
        {
            public string Code;
            public bool Success;
            public CompilationError[] Errors;
        } 
        
        public class CompilationError
        {
            public string Message;
            public int Line;
            public string[] Code;
            public string Severity;
        }

        public static CompilationResult Compile<T>(string codeToCompile, string activateObject, out T activatedObject) {
            Write("Let's compile!");
            Write("Parsing the code into the SyntaxTree");

            codeToCompile = @"
using Ei;
using Ei.Core;
using Ei.Core.Ontology;
using Ei.Core.Ontology.Actions;
using Ei.Core.Ontology.Transitions;
using Ei.Core.Runtime;
using Ei.Core.Runtime.Planning;
using System;
using System.Collections.Generic;

" + codeToCompile;

            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToCompile);

            // list
            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            var neededAssemblies = new[]
            {
          "System.Runtime",
          "System.Collections",
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
            
            // project based references
            references.Add(MetadataReference.CreateFromFile(typeof(IPhysiologyStore).GetTypeInfo().Assembly.Location));

            // Console.WriteLine(string.Join("\n", references.Select(s => s.Display).ToArray()));

            Write("Compiling ...");
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream()) {
                activatedObject = default(T);

                EmitResult result = compilation.Emit(ms);

                if (!result.Success) {
                    Write("Compilation failed!");
                    var source = codeToCompile.Split('\n');
                    var compilationResult = new CompilationResult {
                        Code = codeToCompile,
                        Success = false
                    };
                    for (int i = 0; i < source.Length; i++) {
                        Console.WriteLine(i.ToString().PadLeft(5) + ": " + source[i]);
                    }
                    
                    Console.WriteLine("======================================");
                    
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    var list = failures.Select(f => {
                        var span = f.Location.GetMappedLineSpan();
                        var line = span.StartLinePosition.Line;
                        var code = new string[10];
                        var m = 0;
                        for (var i = line - 5; i < line + 5; i++) {
                            if (i == source.Length) {
                                break;
                            }
                            code[m++] = source[i] + ((i == line) ? "  // <- ERROR" : "");
                        }
                        
                        Console.Error.WriteLine(f.ToString());
                        
                        return new CompilationError {
                            Severity = f.Severity.ToString(),
                            Code = code,
                            Line = line,
                            Message = f.GetMessage()
                        };
                    });


                    compilationResult.Errors = list.ToArray();
                    return compilationResult;
                }
                else if (activateObject != null) {
                    Write("Compilation successful! Now instantiating and executing the code ...");
                    ms.Seek(0, SeekOrigin.Begin);

                    Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                    var type = assembly.GetType(activateObject);
                    activatedObject = (T)assembly.CreateInstance(activateObject);
                }
                
                return new CompilationResult {
                    Success = true,
                    Code = codeToCompile,
                };
            }
        }
    }
}