﻿using ComparableGenerator.UnitTest.Verifiers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HelloSourceGenerator.Test
{
    public class HelloAnalyzerTest
    {
        [Fact]
        public async Task ToStringが実装済みのときエラーが通知されCodeFixが提案される()
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
            var expected = CSharpAnalyzerVerifier<HelloDiagnosticAnalyzer>
                .Diagnostic(HelloDiagnosticAnalyzer.ToStringIsImplemented)
                .WithLocation(13, 32)
                .WithArguments("Program");

            var fixedSource = @"
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
";

            await CSharpCodeFixVerifier<HelloDiagnosticAnalyzer, HelloCodeFixProvider>
                .VerifyCodeFixAsync(source, expected, fixedSource);
        }

        [Fact]
        public async Task ToStringが未実装のとき通知が発生しない()
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
    }
}
";

            await CSharpAnalyzerVerifier<HelloDiagnosticAnalyzer>.VerifyAnalyzerAsync(source);
        }
    }
}
