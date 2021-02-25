using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HelloSourceGenerator
{
    [Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        private const string Id = "HW0001";
        private static readonly DiagnosticDescriptor ClassNameIsNotProgram = new DiagnosticDescriptor(
            Id,
            "Class name is not Program",
            "'{0}' is not Program",
            "Usage",
            DiagnosticSeverity.Warning,
            true);

        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!System.Diagnostics.Debugger.IsAttached)
//            {
//                System.Diagnostics.Debugger.Launch();
//            }
//#endif
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var syntaxReceiver = (SyntaxReceiver)context.SyntaxReceiver;

            foreach (var classDeclarationSyntax in syntaxReceiver.Classes)
            {
                if (classDeclarationSyntax.Identifier.Text == "Program")
                {
                    var namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)classDeclarationSyntax.Parent;
                    var identifierNameSyntax = (IdentifierNameSyntax)namespaceDeclarationSyntax.Name;

                    var program = new Program
                    {
                        Namespace = identifierNameSyntax.Identifier.Text,
                        Files = context.Compilation.SyntaxTrees.Select(x => x.FilePath)
                    };
                    context.AddSource("Program.SayHello.cs", program.TransformText());
                }
                else
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            ClassNameIsNotProgram, 
                            classDeclarationSyntax.GetLocation(), 
                            classDeclarationSyntax.Identifier.Text));
                }
            }
        }

        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    Classes.Add(classDeclarationSyntax);
                }
            }
        }

    }
}
