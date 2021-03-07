using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace HelloSourceGenerator
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HelloWorldAnalyzer : DiagnosticAnalyzer
    {
        public const string ToStringIsImplementedId = "HW0001";

        private const string Usage = "Usage";

        public static readonly DiagnosticDescriptor ToStringIsImplemented =
            new DiagnosticDescriptor(
                id: ToStringIsImplementedId,
                title: AnalyzerResources.Title,
                messageFormat: AnalyzerResources.MessageFormat,
                category: Usage,
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ToStringIsImplemented);

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclarationNode, SyntaxKind.MethodDeclaration);
        }

        private void AnalyzeMethodDeclarationNode(SyntaxNodeAnalysisContext context)
        {
            var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;

            if (methodDeclarationSyntax.Identifier.Text != "ToString") return;

            var typeDeclarationSyntax = (TypeDeclarationSyntax)methodDeclarationSyntax.Parent;

            context.ReportDiagnostic(
                Diagnostic.Create(
                    ToStringIsImplemented,
                    methodDeclarationSyntax.Identifier.GetLocation(),
                    typeDeclarationSyntax.Identifier.Text));
        }
    }
}
