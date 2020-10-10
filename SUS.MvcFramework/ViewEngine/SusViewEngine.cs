using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SUS.MvcFramework.ViewEngine
{
    public class SusViewEngine : IViewEngine
    {
        public object CHarpCompilation { get; private set; }

        public string GetHtml(string templateCode, object viewModel)
        {
            var csharpCode = GenerateCSharpFromTemplate(templateCode);
            var executableObj = GenerateExecutableCode(csharpCode, viewModel);
            var html = executableObj.ExecuteTamplate(viewModel);

            return html;

        }

        private string GenerateCSharpFromTemplate(string templateCode)
        {
            string methodBody = GetMethodBody(templateCode);

            string csharpCode = @"
using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using SUS.MvcFramework.ViewEngine;

namespace ViewNamespace
{
    public class ViewClass : IView
    {
        public string ExecuteTamplate(object viewModel)
        {
            var html = new StringBuilder();

            " + methodBody + @"

            return html.ToString();
            
        }
    }
}
";

            return csharpCode;
        }

        private string GetMethodBody(string templateCode)
        {
            throw new NotImplementedException();
        }

        private IView GenerateExecutableCode(string csharpCode, object viewModel)
        {
            var compileResult = CSharpCompilation.Create("ViewAssembly")
                .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddReferences(MetadataReference.CreateFromFile(typeof(IView).Assembly.Location));
            if (viewModel != null)
            {
                compileResult = compileResult.AddReferences(MetadataReference.CreateFromFile(viewModel.GetType().Assembly.Location));
            }

            var libraries = Assembly.Load(new AssemblyName("netstandart")).GetReferencedAssemblies();

            foreach (var lib in libraries)
            {
                compileResult = compileResult.AddReferences(MetadataReference.CreateFromFile(Assembly.Load(lib).Location));
            }

            compileResult = compileResult.AddSyntaxTrees(SyntaxFactory.ParseSyntaxTree(csharpCode));

            using (MemoryStream memoryStream = new MemoryStream()) 
            { 
                EmitResult result = compileResult.Emit(memoryStream);

                if (!result.Success)
                {
                    //Compile errors
                    return new ErrorView(result.Diagnostics
                        .Where(x => x.Severity == DiagnosticSeverity.Error)
                        .Select(x => x.GetMessage()), csharpCode);
                }

                memoryStream.Seek(0, SeekOrigin.Begin);
                var byteAssembly = memoryStream.ToArray();
                var assembly = Assembly.Load(byteAssembly);
                var viewType = assembly.GetType("ViewNamespace.ViewClass");
                var instance = Activator.CreateInstance(viewType);

                return instance as IView;

            }




            return null;
        }

        
    }
}
