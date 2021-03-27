using System;

namespace HelloSourceGeneratorConsoleApp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(new Program().ToString());
            Console.WriteLine(new Foo().ToString());
        }
    }

    partial class Foo
    {
    }
}
