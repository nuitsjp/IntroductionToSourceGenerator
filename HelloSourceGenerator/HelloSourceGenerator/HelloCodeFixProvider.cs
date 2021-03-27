using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Composition;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelloSourceGenerator
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(HelloCodeFixProvider)), Shared]
    public class HelloCodeFixProvider : CodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(HelloDiagnosticAnalyzer.ToStringIsImplementedId);

        public sealed override FixAllProvider GetFixAllProvider()
        {
            return WellKnownFixAllProviders.BatchFixer;
        }

        public override Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            foreach (var diagnostic in context.Diagnostics)
            {
                switch (diagnostic.Id)
                {
                    case HelloDiagnosticAnalyzer.ToStringIsImplementedId:
                        FixToStringIsImplemented(context, diagnostic);
                        break;
                    default:
                        throw new NotImplementedException($"{diagnostic.Id} is not implemented.");
                }
            }

            return Task.CompletedTask;
        }

        private void FixToStringIsImplemented(CodeFixContext context, Diagnostic diagnostic)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            context.RegisterCodeFix(
                CodeAction.Create(
                    title: AnalyzerResources.CodeFixTitle,
                    c => AppendComparableAsync(context, diagnostic, c),
                    equivalenceKey: HelloDiagnosticAnalyzer.ToStringIsImplementedId),
                diagnostic);
        }

        private async Task<Solution> AppendComparableAsync(
            CodeFixContext context, 
            Diagnostic diagnostic,
            CancellationToken cancellationToken)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            var declaration = root.FindToken(diagnostic.Location.SourceSpan.Start);
            var methodDeclarationSyntax = (MethodDeclarationSyntax)declaration.Parent;

            return context
                .Document
                .WithSyntaxRoot(root.RemoveNode(methodDeclarationSyntax, SyntaxRemoveOptions.KeepNoTrivia))
                .Project
                .Solution;
        }
    }
}
