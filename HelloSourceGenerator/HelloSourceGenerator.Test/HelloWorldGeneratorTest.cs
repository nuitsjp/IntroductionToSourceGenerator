using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Xunit.Sdk;

namespace HelloSourceGenerator.Test
{
    public class HelloWorldGeneratorTest
    {
        [Fact]
        public void ProgramクラスにToStringメソッドがオーバーライドされる()
        {
            var input = CSharpSyntaxTree.ParseText(@"using System;

namespace MyNamespace
{
    partial class Program
    {
    }
}
", path: "C:\\Program.cs");

            var inputCompilation = CSharpCompilation.Create("compilation", new[] { input });
            var generator = new HelloWorldGenerator();
            var driver = CSharpGeneratorDriver.Create(generator);

            var runResult = driver
                .RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _)
                .GetRunResult();

            Assert.Single(runResult.GeneratedTrees);

            var expected = CSharpSyntaxTree.ParseText(@"namespace MyNamespace
{
    partial class Program
    {
        public override string ToString()
        {
            return ""Hello, Source Generator! by Program"";
        }
    }
}");
            Equal(expected, runResult.GeneratedTrees.Single());
        }

        private static void Equal(SyntaxTree expected, SyntaxTree actual)
        {
            var diff = expected.GetChanges(actual);
            if (!diff.Any()) return;

            var detail = string.Join(". ", diff.Select(x => x.ToString()));
            var userMessage = $"Generated SyntaxTree differs from the expected one. {detail}";
            throw new AssertActualExpectedException(
                expected,
                actual,
                userMessage,
                "Expected SyntaxTree",
                "Actual SyntaxTree");
        }

    }
}
