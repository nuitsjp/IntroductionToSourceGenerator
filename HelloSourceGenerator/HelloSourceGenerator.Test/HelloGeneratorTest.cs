using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;
using Xunit.Sdk;

namespace HelloSourceGenerator.Test
{
    public class HelloGeneratorTest
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
");

            var inputCompilation = CSharpCompilation.Create("compilation", new[] { input });
            var generator = new HelloGenerator();
            var driver = CSharpGeneratorDriver.Create(generator);

            GeneratorDriverRunResult runResult = driver
                .RunGenerators(inputCompilation)
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

        [Fact]
        public void ToStringが実装済みのときコードが追加されない()
        {
            var input = CSharpSyntaxTree.ParseText(@"using System;

namespace MyNamespace
{
    partial class Program
    {
        public override string ToString() => string.Empty;
    }
    }
");

            var inputCompilation = CSharpCompilation.Create("compilation", new[] { input });
            var generator = new HelloGenerator();
            var driver = CSharpGeneratorDriver.Create(generator);

            GeneratorDriverRunResult runResult = driver
                .RunGenerators(inputCompilation)
                .GetRunResult();

            Assert.Empty(runResult.GeneratedTrees);
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
