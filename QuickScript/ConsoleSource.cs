using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using QuickScript.Utils;
using QuickScript.Exporters;

namespace QuickScript
{
    internal class ConsoleSource
    {
        public class Employee
        {
            public HashString Name { get; set; } = new HashString("Whatever");
            public Employee? Manager { get; set; }
            public List<Employee>? DirectReports { get; set; }
        }
        static public void Main(string[] args)
        {
            QuickscriptSettings settings = new QuickscriptSettings();

            IExporter exporter = new JSonExporter();
            DataMap dm = DataMap.ConstructDataMap(settings.DataMapPath);
            dm.AssimilateTypeInstanceDescriptions(Parser.ParseDirectory(settings.QuickScriptsDirectory));
            dm.SaveToDisk(settings.DataMapPath);

            exporter.Export(new ExportSettings(), Parser.ParseDirectory(settings.QuickScriptsDirectory));
        }
    }
}
