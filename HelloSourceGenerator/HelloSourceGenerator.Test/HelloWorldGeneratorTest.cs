using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Xunit;
using Xunit.Sdk;

namespace HelloSourceGenerator.Test
{
    public class HelloWorldGeneratorTest
    {
        [Fact]
        public void ProgramクラスにSayHelloメソッドが追加される()
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

            var expected = CSharpSyntaxTree.ParseText(@"using System;

namespace MyNamespace
{
    partial class Program
    {
        public static void SayHello()
        {
            Console.WriteLine(""Hello, T4 Template with Source Generator!"");
            Console.WriteLine("" - C:\\Program.cs"");
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

        [Fact]
        public void FooクラスにSayHelloメソッドが追加されない()
        {
            var input = CSharpSyntaxTree.ParseText(@"using System;

namespace MyNamespace
{
    partial class Foo
    {
    }
}
");

            var inputCompilation = CSharpCompilation.Create("compilation", new[] { input });

            var generator = new HelloWorldGenerator();
            var driver = CSharpGeneratorDriver.Create(generator);
            var runResult = driver
                .RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _)
                .GetRunResult();

            Assert.Empty(runResult.GeneratedTrees);
        }
    }
}
