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
            QuickscriptSettings settings = new QuickscriptSettings(); 
            Console.WriteLine(exporter.Export(Parser.ParseDirectory(settings.QuickScriptsDirectory)));

        }
    }
}
