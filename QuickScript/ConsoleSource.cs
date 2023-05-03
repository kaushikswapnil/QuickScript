using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript
{
    internal class ConsoleSource
    {
        static public void Main(string[] args)
        {
            Exporter exporter = new Exporter();
            Console.WriteLine(exporter.Export(Parser.Parse("[whatever, hello]" +
                "\nNewPuru\n" +
                "{\n" +
                "[kya]\n" +
                "int new_mem = 10;\n" +
                "}")));

        }
    }
}
