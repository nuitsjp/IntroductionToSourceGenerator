using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelloSourceGenerator
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HelloWorldFixProvider)), Shared]
    public class HelloWorldFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(HelloWorldAnalyzer.ToStringIsImplementedId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;

                var declaration = root.FindToken(diagnosticSpan.Start);
                var methodDeclarationSyntax = (MethodDeclarationSyntax)declaration.Parent;

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "ToStringを削除する。",
                        c => AppendComparableAsync(context.Document, methodDeclarationSyntax, c),
                        equivalenceKey: nameof(HelloWorldAnalyzer.ToStringIsImplementedId)),
                    diagnostic);
            }

        }

        private async Task<Solution> AppendComparableAsync(Document document,
            MethodDeclarationSyntax typeDeclaration,
            CancellationToken cancellationToken)
        {
            var root = await document.GetSyntaxRootAsync(cancellationToken);
            return document
                .WithSyntaxRoot(root.RemoveNode(typeDeclaration, SyntaxRemoveOptions.KeepNoTrivia))
                .Project
                .Solution;
        }
    }
}
