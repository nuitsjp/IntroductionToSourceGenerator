using System.Collections.Generic;

namespace HelloSourceGenerator
{
    partial class Program
    {
        public string Namespace { get; set; }
        public IEnumerable<string> Files { get; set; }
    }
}