using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Schema;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HelloSourceGenerator
{
    [Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        private const string Id = "HW0001";
        private static readonly DiagnosticDescriptor DeclaredOtherThanPartial = new DiagnosticDescriptor(
            Id,
            "Program class can only be declared partial",
            "'{0}' has been declared",
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
            var syntaxReceiver = (SyntaxReceiver) context.SyntaxReceiver;

            foreach (var classDeclarationSyntax in syntaxReceiver.Classes)
            {
                var namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)classDeclarationSyntax.Parent;
                var identifierNameSyntax = (IdentifierNameSyntax)namespaceDeclarationSyntax.Name;
                var namespaceName = identifierNameSyntax.Identifier.Text;
                var typeName = classDeclarationSyntax.Identifier.Text;

                var source = $@"
using System;

namespace {namespaceName}
{{
    partial class {typeName}
    {{
        public static void SayHello()
        {{
            Console.WriteLine(""Hello, Source Generator by {typeName}!"");
        }}
    }}
}}";
                context.AddSource($"{typeName}.SayHello.cs", source);
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
