using System.Collections.Generic;
using System.Linq;
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

            foreach (var classDeclarationSyntax in syntaxReceiver.Classes)
            {
                var namespaceDeclarationSyntax = (NamespaceDeclarationSyntax) classDeclarationSyntax.Parent;
                var identifierNameSyntax = (IdentifierNameSyntax) namespaceDeclarationSyntax.Name;
                var namespaceName = identifierNameSyntax.Identifier.Text;
                var typeName = classDeclarationSyntax.Identifier.Text;

                var source = $@"namespace {namespaceName}
{{
    partial class {typeName}
    {{
        public override string ToString()
        {{
            return ""Hello, Source Generator! by {typeName}"";
        }}
    }}
}}";
                context.AddSource($"{typeName}.g.cs", source);
            }
        }

        class SyntaxReceiver : ISyntaxReceiver
        {
            public List<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax)
                {
                    if (classDeclarationSyntax
                        .Members
                        .OfType<MethodDeclarationSyntax>()
                        .Any(x => x.Identifier.Text == "ToString"))
                    {
                        return;
                    }

                    Classes.Add(classDeclarationSyntax);
                }
            }
        }

    }
}