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
            QuickscriptSettings settings = new QuickscriptSettings();

            IExporter exporter = new ConsoleExporter();
            DataMap dm = DataMap.ConstructDataMap(settings.DataMapPath);
            dm.SaveToDisk(settings.DataMapPath);            

            exporter.Export(Parser.ParseDirectory(settings.QuickScriptsDirectory));

        }
    }
}
