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
        public JsonSerializationTest(List<T> test_items_list, bool log = true)
        {
            RunTests(test_items_list, log);
        }

        public JsonSerializationTest(T test_item, bool log = true)
        {
            RunTests(new List<T> { test_item }, log);
        }

        private void RunTests(List<T> test_items_list, bool log = true)
        {
            void LogIfAllowed(string message)
            {
                if (log)
                {
                    Console.WriteLine(message);
                }
            }
            LogIfAllowed("-----JSON Serialization Test : " + typeof(T).FullName + " Beginning-----\n");

            for (int i = 0; i < test_items_list.Count; ++i)
            {
                T test_item = test_items_list[i];
                LogIfAllowed("Test #" + i + "\n");

                LogIfAllowed("Serializing object " + test_item);

                var options = JSonExportUtils.GetSerializationOptions();
                string export = JsonSerializer.Serialize<T>(test_item, options);
                LogIfAllowed("\nExporting as " + export);

                T deserialized = JsonSerializer.Deserialize<T>(export, options);
                LogIfAllowed("\nDeserialized as " + deserialized);

                Assertion.SoftAssert(deserialized.Equals(test_item), "Serialization and deserialization should result in same object");
            }

            LogIfAllowed("-----JSON Serialization Test : " + typeof(T).FullName + " End-----\n");
        }
    }
    public class JsonSerializationTests
    {
        public JsonSerializationTests(bool log = true)
        {
            JsonSerializationTest<HashString> test_hs = 
                new JsonSerializationTest<HashString>(new List<HashString>{ new HashString("HelloWorld") }, log);
            JsonSerializationTest<AttributeDefinition> test_ad = 
                new JsonSerializationTest<AttributeDefinition>(new AttributeDefinition(new HashString("FilePath"), 1, new HashString("string")), log);
            JsonSerializationTest<TypeDefinition> test_td = new JsonSerializationTest<TypeDefinition>(new TypeDefinition(new HashString("int")), log);
            JsonSerializationTest<DataMap> test_dm = new JsonSerializationTest<DataMap>(new DataMap(), log);
        }
    }
}
