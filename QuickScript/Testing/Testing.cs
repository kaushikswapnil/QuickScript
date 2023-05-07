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
    public abstract class Test
    { 
        protected bool Log { get; set; }
        protected uint Indent { get; set; }
        protected void LogIfAllowed(string message)
        {
            if (Log)
            {
                const uint IndentSize = 2;
                string final_message = new string(' ', (int)(Indent * IndentSize)) + message;
                Console.WriteLine(message);
            }
        }

        public Test(bool log= true, uint indent = 0) {  Log = log; Indent = indent; }
    }
    public class JsonSerializationTest<T> : Test
    {
        public JsonSerializationTest(List<T> test_items_list, bool log = true, uint indent = 0) : base(log, indent)
        {
            RunTests(test_items_list);
        }

        public JsonSerializationTest(T test_item, bool log = true, uint indent = 0) : base(log, indent)
        {
            RunTests(new List<T> { test_item });
        }

        private void RunTests(List<T> test_items_list)
        {
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

                bool passed = deserialized.Equals(test_item);
                LogIfAllowed("\nTest " + i + (passed ? " PASSED" : " FAILED"));
                Assertion.SoftAssert(passed, "Serialization and deserialization should result in same object");
            }

            LogIfAllowed("-----JSON Serialization Test : " + typeof(T).FullName + " End-----\n");
        }
    }
    public class JsonSerializationTests : Test
    {
        public JsonSerializationTests(bool log = true, uint indent = 0) : base(log, indent)
        {
            JsonSerializationTest<HashString> test_hs =
                new JsonSerializationTest<HashString>(new List<HashString> { new HashString("HelloWorld") }, log);
            JsonSerializationTest<AttributeDefinition> test_ad =
                new JsonSerializationTest<AttributeDefinition>(new AttributeDefinition(new HashString("FilePath"), 1, new HashString("string")), log);
            JsonSerializationTest<TypeDefinition> test_td = new JsonSerializationTest<TypeDefinition>(new TypeDefinition(new HashString("int")), log);
            JsonSerializationTest<DataMap> test_dm = new JsonSerializationTest<DataMap>(new DataMap(), log);
        }
    }

    public class ValidateDataMap : Test
    {
        public ValidateDataMap(DataMap dm, bool log = true, uint indent = 0) : base(log, indent) 
        { 
            //dm.
        }
    }

    public class CreateAndSerializeTestDataMap : Test
    {
        public CreateAndSerializeTestDataMap(bool initial_basic = true, bool log = true, uint indent = 0) : base(log, indent)
        {
            LogIfAllowed("-----CreateAndSerializeTestDataMap Test : Beginning-----\n");
            QuickscriptSettings settings = new QuickscriptSettings();

            DataMap dm = DataMap.ConstructDataMap(settings.DataMapPath, initial_basic);
            dm.AssimilateTypeInstanceDescriptions(Parser.ParseDirectory(settings.QuickScriptsDirectory));

            LogIfAllowed("Constructed datamap as " + dm);

            LogIfAllowed("Attempting dm serialization");
            JsonSerializationTest<DataMap> test_dm = new JsonSerializationTest<DataMap>(dm, log, ++indent);
            LogIfAllowed("Saving to dm");
            dm.SaveToDisk(settings.DataMapPath);

            LogIfAllowed("Attempting reconstruction from disk save path " + settings.DataMapPath);
            DataMap new_dm = DataMap.ConstructDataMap(settings.DataMapPath);

            LogIfAllowed("Constructed new dm as " + new_dm);
            bool passed = dm.Equals(new_dm);
            LogIfAllowed("\nTest " + (passed ? "PASSED" : "FAILED"));
            Assertion.SoftAssert(passed, "Construction from disk should result in same DM!");

            LogIfAllowed("-----CreateAndSerializeTestDataMap Test : End-----\n");
        }
    }

    public class TestsSuite : Test
    {
        public TestsSuite(bool log = true, uint indent = 0) : base(log, indent)
        { 
            JsonSerializationTests test_json = new JsonSerializationTests(log, indent);
            CreateAndSerializeTestDataMap test_dm_create = new CreateAndSerializeTestDataMap(true, log, indent);
        }
    }
}
