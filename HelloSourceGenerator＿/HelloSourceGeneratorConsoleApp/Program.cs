using System;

namespace HelloSourceGeneratorConsoleApp
{
    partial class Program
    {
        static void Main(string[] args)
        {
            SayHello();
            Foo.SayHello();
        }
    }

    public partial class Foo
    {

    }
}
