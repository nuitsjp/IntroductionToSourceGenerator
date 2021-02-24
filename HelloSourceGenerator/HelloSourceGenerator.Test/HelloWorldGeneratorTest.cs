using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

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
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
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

            Assert.Empty(expected.GetChanges(runResult.GeneratedTrees.Single()));
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
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            var runResult = driver
                .RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _)
                .GetRunResult();

            Assert.Empty(runResult.GeneratedTrees);
        }
    }
}
