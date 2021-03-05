using System.Text;
using Microsoft.CodeAnalysis;

namespace HelloSourceGenerator
{
    [Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var builder = new StringBuilder(@"
using System;

namespace HelloSourceGenerator
{
    public static class HelloWorld
    {
        public static void SayHello()
        {
            Console.WriteLine(""Hello, Source Generator!"");
        }
    }
}");
            context.AddSource("HelloWorld.cs", builder.ToString());
        }
    }
}