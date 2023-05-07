using QuickScript.Exporters;
using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuickScript.Testing
{
    public class JsonSerializationTest<T>
    {
        public JsonSerializationTest(List<T> test_items_list)
        {
            RunTests(test_items_list);
        }

        public JsonSerializationTest(T test_item)
        {
            RunTests(new List<T> { test_item });
        }

        private void RunTests(List<T> test_items_list)
        {
            Console.WriteLine("-----JSON Serialization Test : " + typeof(T).FullName + " Beginning-----\n");

            for (int i = 0; i < test_items_list.Count; ++i)
            {
                T test_item = test_items_list[i];
                Console.WriteLine("Test #" + i + "\n");

                Console.WriteLine("Serializing object " + test_item);

                var options = JSonExportUtils.GetSerializationOptions();
                string export = JsonSerializer.Serialize<T>(test_item, options);
                Console.WriteLine("\nExporting as " + export);

                T deserialized = JsonSerializer.Deserialize<T>(export, options);
                Console.WriteLine("\nDeserialized as " + deserialized);

                Assertion.SoftAssert(deserialized.Equals(test_item), "Serialization and deserialization should result in same object");
            }

            Console.WriteLine("-----JSON Serialization Test : " + typeof(T).FullName + " End-----\n");
        }
    }
    public class JsonSerializationTests
    {
        public JsonSerializationTests()
        {
            JsonSerializationTest<HashString> test_hs = 
                new JsonSerializationTest<HashString>(new List<HashString>{ new HashString("HelloWorld") });
            JsonSerializationTest<AttributeDefinition> test_ad = 
                new JsonSerializationTest<AttributeDefinition>(new AttributeDefinition(new HashString("FilePath"), 1, new HashString("string")));
            JsonSerializationTest<TypeDefinition> test_td = new JsonSerializationTest<TypeDefinition>(new TypeDefinition(new HashString("int")));
            JsonSerializationTest<DataMap> test_dm = new JsonSerializationTest<DataMap>(new DataMap());
        }
    }
}
