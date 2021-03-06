using ComparableGenerator.UnitTest.Verifiers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HelloSourceGenerator.Test
{
    public class HelloWorldAnalyzerTest
    {
        [Fact]
        public async Task AnalyzeAsync()
        {
            var source = @"
using System;

namespace HelloSourceGeneratorConsoleApp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new Program().ToString());
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
";
            var diagnostic = CSharpAnalyzerVerifier<HelloWorldAnalyzer>
                .Diagnostic(HelloWorldAnalyzer.Rule)
                .WithLocation(13, 32)
                .WithArguments("Program");

            await CSharpCodeFixVerifier<HelloWorldAnalyzer, HelloWorldFixProvider>.VerifyCodeFixAsync(
                source,
                new[] { diagnostic },
                @"
using System;

namespace HelloSourceGeneratorConsoleApp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new Program().ToString());
        }
    }
}
");

        }
    }
}
