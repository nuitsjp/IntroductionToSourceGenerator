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

namespace HelloSourceGeneratorConsoleApp
{
    partial class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
", path: "C:\\Program.cs");

            var expected = CSharpSyntaxTree.ParseText(@"using System;

namespace HelloSourceGeneratorConsoleApp
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

            var inputCompilation = CSharpCompilation.Create("compilation",
                new[] { input },
                new[] { MetadataReference.CreateFromFile(typeof(int).GetTypeInfo().Assembly.Location) },
                new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            var generator = new HelloWorldGenerator();
            GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
            var runResult = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out _, out _).GetRunResult();

            Assert.Single(runResult.GeneratedTrees);
            Assert.Empty(expected.GetChanges(runResult.GeneratedTrees.Single()));
        }
    }
}
