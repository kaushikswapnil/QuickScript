using QuickScript.Exporters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuickScript.Testing
{
    public class JsonSerializationTest<T>
    {
        public JsonSerializationTest(T test_item)
        {
            Console.WriteLine("-----JSON Serialization Test : " + typeof(T).FullName + "-----\n");
            Console.WriteLine("Testing serialization\n");
            Console.WriteLine("Serializing object " + test_item);

            var options = JSonExportUtils.GetSerializationOptions();
            string export = JsonSerializer.Serialize<T>(test_item, options);
            Console.WriteLine("\nExporting as " + export);

            T deserialized = JsonSerializer.Deserialize<T>(export, options);
            Console.WriteLine("\nDeserialized as " + deserialized);
        }
    }
}
