using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using QuickScript.Utils;

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

            //IExporter exporter = new JSonExporter();
            DataMap dm = DataMap.ConstructDataMap(settings.DataMapPath);
            dm.SaveToDisk(settings.DataMapPath);

            //exporter.Export(new ExportSettings(), Parser.ParseDirectory(settings.QuickScriptsDirectory));


            Employee tyler = new Employee();

            Employee adrian = new()
            {
                Name = HashString.FromString("Adrian King")
            };

            tyler.DirectReports = new List<Employee> { adrian };
            adrian.Manager = tyler;

            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true,
                IncludeFields = true,
                Converters= { new JSonExporter.HashStringConverter() }
            };

            string tylerJson = JsonSerializer.Serialize<HashString>(tyler.Name, options);
            var des = JsonSerializer.Deserialize<HashString>(tylerJson, options);
            Console.WriteLine($"Tyler serialized:\n{tylerJson}");
        }
    }
}
