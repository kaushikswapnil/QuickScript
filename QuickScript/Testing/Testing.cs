using QuickScript.Exporters;
using QuickScript.Typing;
using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace QuickScript.Testing
{
    public abstract class Test
    { 
        protected bool Log { get; set; }
        protected uint Indent { get; set; }
        protected string Name;
        protected void LogIfAllowed(string message, uint indent)
        {
            if (Log)
            {
                const uint IndentSize = 2;
                string final_message = new string(' ', (int)(indent * IndentSize)) + message;
                Console.WriteLine(message);
            }
        }
        protected void LogIfAllowed(string message)
        {
            LogIfAllowed(message, Indent);
        }

        protected void LogTestBeginIfAllowed()
        {
            LogIfAllowed("\n-----" + Name + " Begin-----\n");
        }
        protected void LogTestEndIfAllowed()
        {
            LogIfAllowed("\n-----" + Name + " End-----\n");
        }

        protected void LogTestPassedIfAllowed(bool passed)
        {
            if (passed)
            {
                LogIfAllowed("---Test " + Name + ((passed) ? "PASSED" : "FAILED") + "---");
            }
        }

        public Test(bool log= true, uint indent = 0, string name = "<MISSING FIELD. ENTER AT TEST CONSTRUCTOR>") {  Log = log; Indent = indent; Name = name; }
    
        static public DataMap GetTestDataMap(bool force_initial_basic = true, bool parse_qs_dir = true)
        {
            DataMap dm = DataMap.ConstructDataMap(QuickscriptSettings.DEFAULT_DATA_MAP_PATH, force_initial_basic);
            if (parse_qs_dir)
                dm.AssimilateTypeInstanceDescriptions(Parser.ParseDirectory(QuickscriptSettings.DEFAULT_INPUT_TEST_DIRECTORY));

            return dm;
        }
    }
    public class JsonSerializationTest<T> : Test
    {
        public JsonSerializationTest(List<T> test_items_list, bool log = true, uint indent = 0) : base(log, indent, "JsonSerializationTest<" + typeof(T).FullName +">")
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
        public JsonSerializationTests(bool log = true, uint indent = 0) : base(log, indent, typeof(JsonSerializationTests).Name)
        {
            JsonSerializationTest<HashString> test_hs =
                new JsonSerializationTest<HashString>(new List<HashString> { new HashString("HelloWorld") }, log);
            JsonSerializationTest<AttributeDefinition> test_ad =
                new JsonSerializationTest<AttributeDefinition>(new AttributeDefinition(new HashString("FilePath"), 1, new HashString("string")), log);
            JsonSerializationTest<TypeDefinition> test_td = 
                new JsonSerializationTest<TypeDefinition>(new TypeDefinition(new HashString("int")), log);
            JsonSerializationTest<DataMap> test_dm_basic = new JsonSerializationTest<DataMap>(GetTestDataMap(true, false), log);
            JsonSerializationTest<DataMap> test_dm_parsed = new JsonSerializationTest<DataMap>(GetTestDataMap(true, true), log);
        }
    }

    public class ValidateDataMap : Test
    {
        public ValidateDataMap(DataMap dm, bool log = true, uint indent = 0) : base(log, indent, typeof(ValidateDataMap).Name) 
        {
            Test(dm, log, indent);
        }
        private void Test(DataMap dm, bool log, uint indent)
        {
            LogTestBeginIfAllowed();

            bool passed = true;

            LogIfAllowed("Testing attributes");
            List<AttributeDefinition> basic_attr_list = TypeInformationUtils.SupportedAttributeDefinitionList();
            List<AttributeDefinition> attributes_in_dm = dm.AttributeDefinitions;
            for (int attr_iter = 0; attr_iter < attributes_in_dm.Count; ++attr_iter)
            {
                AttributeDefinition attr_in_dm = attributes_in_dm[attr_iter];
                for (int next_iter = attr_iter + 1; next_iter < attributes_in_dm.Count; ++next_iter)
                {
                    AttributeDefinition next_item = attributes_in_dm[next_iter];
                    passed &= next_item.Name.Equals(attr_in_dm.Name) == false;
                    Assertion.SoftAssert(next_item.Name.Equals(attr_in_dm.Name) == false,
                        "This should have been a unique item " + attr_in_dm.Name.AsString());
                }
            }

            foreach (AttributeDefinition basic_attr in basic_attr_list)
            {
                AttributeDefinition attr_in_dm_matching_name = attributes_in_dm.Find(x => x.Name == basic_attr.Name);
                passed &= attr_in_dm_matching_name != null;
                Assertion.SoftAssert(attr_in_dm_matching_name != null, "We should have one attribute with a matching name for " + basic_attr.Name.AsString());
            }

            LogIfAllowed("Testing type defs");
            List<TypeDefinition> basic_types_list = TypeInformationUtils.SupportedBasicTypeDefinitionsList();
            List<TypeDefinition> types_in_dm = dm.TypeDefinitions;
            for (int type_iter = 0; type_iter < attributes_in_dm.Count; ++type_iter)
            {
                TypeDefinition type_in_dm = types_in_dm[type_iter];
                for (int next_iter = type_iter + 1; next_iter < types_in_dm.Count; ++next_iter)
                {
                    TypeDefinition next_item = types_in_dm[next_iter];
                    passed &= next_item.Name.Equals(type_in_dm.Name) == false;
                    Assertion.SoftAssert(next_item.Name.Equals(type_in_dm.Name) == false,
                        "This should have been a unique item " + type_in_dm.Name.AsString());
                }
            }

            foreach (TypeDefinition basic_type in basic_types_list)
            {
                TypeDefinition type_in_dm_matching_name = types_in_dm.Find(x => x.Name == basic_type.Name);
                passed &= type_in_dm_matching_name != null;
                Assertion.SoftAssert(type_in_dm_matching_name != null, "We should have one type definition with a matching name for " + basic_type.Name.AsString());
            }

            LogTestPassedIfAllowed(passed);

            LogTestEndIfAllowed();
        }
    }

    public class CreateAndSerializeTestDataMap : Test
    {
        public CreateAndSerializeTestDataMap(bool initial_basic = true, bool log = true, uint indent = 0) : base(log, indent)
        {
            LogIfAllowed("-----CreateAndSerializeTestDataMap Test : Beginning-----\n");

            DataMap dm = GetTestDataMap(initial_basic, true);
            LogIfAllowed("Constructed datamap as " + dm);

            LogIfAllowed("Attempting dm serialization");
            JsonSerializationTest<DataMap> test_dm = new JsonSerializationTest<DataMap>(dm, log, ++indent);
            LogIfAllowed("Saving to dm");
            dm.SaveToDisk(QuickscriptSettings.DEFAULT_DATA_MAP_PATH);

            LogIfAllowed("Attempting reconstruction from disk save path " + QuickscriptSettings.DEFAULT_DATA_MAP_PATH);
            DataMap new_dm = DataMap.ConstructDataMap(QuickscriptSettings.DEFAULT_DATA_MAP_PATH);

            LogIfAllowed("Constructed new dm as " + new_dm);
            bool passed = dm.Equals(new_dm);
            LogIfAllowed("\nTest " + (passed ? "PASSED" : "FAILED"));
            Assertion.SoftAssert(passed, "Construction from disk should result in same DM!");

            LogIfAllowed("-----CreateAndSerializeTestDataMap Test : End-----\n");
        }
    }

    public class JsonExporterTest : Test
    {
        public JsonExporterTest(bool log = true, uint indent = 0) : base(log, indent, typeof(JsonExporterTest).Name)
        {
            LogTestBeginIfAllowed();

            DataMap dm = DataMap.ConstructDataMap(QuickscriptSettings.DEFAULT_DATA_MAP_PATH);
            JSonExporter exporter = new JSonExporter();
            exporter.Export(new ExportSettings(), dm);

            LogTestEndIfAllowed();
        }
    }

    public class CppExporterTest : Test
    {
        public CppExporterTest(bool log = true, uint indent = 0) : base(log, indent, typeof(CppExporterTest).Name)
        {
            LogTestBeginIfAllowed();

            DataMap dm = DataMap.ConstructDataMap(QuickscriptSettings.DEFAULT_DATA_MAP_PATH);
            CppExporter exporter = new CppExporter();
            exporter.Export(new ExportSettings(), dm);

            LogTestEndIfAllowed();
        }
    }

    public class TestsSuite : Test
    {
        public TestsSuite(bool log = true, uint indent = 0) : base(log, indent)
        { 
            JsonSerializationTests test_json = new JsonSerializationTests(log, indent);
            CreateAndSerializeTestDataMap test_dm_create = new CreateAndSerializeTestDataMap(true, log, indent);
            ValidateDataMap test_dm_val = new ValidateDataMap(GetTestDataMap(), log, indent);
            JsonExporterTest test_json_export = new JsonExporterTest();
            CppExporterTest test_cpp_export = new CppExporterTest();
        }
    }
}
