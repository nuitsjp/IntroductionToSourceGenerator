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
            if (syntaxReceiver.Program == null) return;

            var namespaceDeclarationSyntax = (NamespaceDeclarationSyntax)syntaxReceiver.Program.Parent;
            var identifierNameSyntax = (IdentifierNameSyntax)namespaceDeclarationSyntax.Name;

            var program = new Program
            {
                Namespace = identifierNameSyntax.Identifier.Text,
                Files = context.Compilation.SyntaxTrees.Select(x => x.FilePath)
            };
            context.AddSource("Program.SayHello.cs", program.TransformText());
        }

        class SyntaxReceiver : ISyntaxReceiver
        {
            public ClassDeclarationSyntax Program { get; set; }

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    if (classDeclarationSyntax.Identifier.Text == "Program")
                    {
                        Program = classDeclarationSyntax;
                    }
                }
            }
        }

    }
}
