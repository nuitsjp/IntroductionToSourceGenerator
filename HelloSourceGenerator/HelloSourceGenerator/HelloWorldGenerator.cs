using System;
using System.Text;
using Microsoft.CodeAnalysis;

namespace HelloSourceGenerator
{
    [Generator]
    public class HelloWorldGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!System.Diagnostics.Debugger.IsAttached)
//            {
//                System.Diagnostics.Debugger.Launch();
//            }
//#endif
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
            Console.WriteLine(""Hello, Source Generator!"");");

            foreach (var syntaxTree in context.Compilation.SyntaxTrees)
            {
                builder.AppendLine($@"Console.WriteLine(@"" - {syntaxTree.FilePath}"");");
                //builder.Append("Console.WriteLine(\" - ");
                //builder.Append(syntaxTree.FilePath.Replace("\\", "\\\\"));
                //builder.Append("\");");
            }

            builder.Append(@"        }
    }
}");
            context.AddSource("HelloWorld.cs", builder.ToString());
        }
    }
}
