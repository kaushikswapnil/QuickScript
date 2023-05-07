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
        public JsonSerializationTest(T test_item)
        {
            Console.WriteLine("-----JSON Serialization Test : " + typeof(T).FullName + "-----\n");
            Console.WriteLine("Serializing object " + test_item);

            var options = JSonExportUtils.GetSerializationOptions();
            string export = JsonSerializer.Serialize<T>(test_item, options);
            Console.WriteLine("\nExporting as " + export);

            T deserialized = JsonSerializer.Deserialize<T>(export, options);
            Console.WriteLine("\nDeserialized as " + deserialized);

            Assertion.SoftAssert(deserialized.Equals(test_item), "Serialization and deserialization should result in same object");
        }
    }
    public class JsonSerializationTests
    {
        public JsonSerializationTests()
        {
            JsonSerializationTest<HashString> test_hs = new JsonSerializationTest<HashString>(new HashString("HelloWorld"));
            JsonSerializationTest<AttributeDefinition> test_ad = new JsonSerializationTest<AttributeDefinition>(new AttributeDefinition(new HashString("FilePath"), 1, new HashString("string")));
            JsonSerializationTest<TypeDefinition> test_td = new JsonSerializationTest<TypeDefinition>(new TypeDefinition(new HashString("int")));
            JsonSerializationTest<DataMap> test_dm = new JsonSerializationTest<DataMap>(new DataMap());
        }
    }
}
