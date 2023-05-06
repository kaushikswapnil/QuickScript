using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;
using static QuickScript.Exporters.JSonExportUtils;

namespace QuickScript.Exporters
{
    static public class JSonExportUtils
    {
        static public HashString ReadHashString(ref Utf8JsonReader reader,
                                            JsonSerializerOptions options)
        {
            Assertion.SoftAssert(reader.TokenType == JsonTokenType.StartObject, "Hashstring should start with a start object");
            
            bool read = reader.Read();
            Assertion.Assert(read, "Error reading hashstring object");
            Assertion.SoftAssert(reader.TokenType == JsonTokenType.PropertyName, "Hashstring should have a property name");

            read = reader.Read();
            Assertion.Assert(read, "Error reading hashstring object");
            Assertion.SoftAssert(reader.TokenType == JsonTokenType.String, "Hashstring should have a string");
            string str = reader.GetString();

            read = reader.Read();
            Assertion.Assert(read, "Error reading hashstring object");
            Assertion.SoftAssert(reader.TokenType == JsonTokenType.EndObject, "Hashstring should end with a end object");
            return new HashString(str);
        }
        static public void WriteHashString(
                Utf8JsonWriter writer,
                HashString hashstring,
                JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Str", hashstring.AsString());
            writer.WriteEndObject();
        }

        static public AttributeDefinition ReadAttributeDefinition(ref Utf8JsonReader reader,
                                    JsonSerializerOptions options)
        {
            Assertion.Assert(reader.TokenType == JsonTokenType.StartObject, "AttributeDefinition should start with a start object");


            bool read = reader.Read();
            Assertion.Assert(read, "Error reading AttributeDefinition object");
            Assertion.Assert(reader.TokenType == JsonTokenType.PropertyName, "Error reading AttributeDefinition object");
            read = reader.Read();
            string str = reader.GetString();
            HashString attr_def_name = new HashString(str);

            read = reader.Read();
            Assertion.Assert(read, "Error reading AttributeDefinition object");
            Assertion.Assert(reader.TokenType == JsonTokenType.PropertyName, "Error reading AttributeDefinition object");
            read = reader.Read();
            Assertion.Assert(reader.TokenType == JsonTokenType.Number, "AttributeDefinition should have a number here");
            int min_val_count = reader.GetInt32();

            read = reader.Read();
            Assertion.Assert(read, "Error reading AttributeDefinition object");
            Assertion.Assert(reader.TokenType == JsonTokenType.PropertyName, "Error reading AttributeDefinition object");
            read = reader.Read();
            Assertion.Assert(reader.TokenType == JsonTokenType.Number, "AttributeDefinition should have a number here");
            int max_val_count = reader.GetInt32();

            read = reader.Read();
            Assertion.Assert(read, "Error reading AttributeDefinition object");
            Assertion.Assert(reader.TokenType == JsonTokenType.PropertyName, "AttributeDefinition should have a string");
            read = reader.Read();
            Assertion.Assert(reader.TokenType == JsonTokenType.String, "AttributeDefinition should have a number here"); 
            str = reader.GetString();
            HashString attr_val_type_name = new HashString(str);

            read = reader.Read();
            Assertion.Assert(read, "Error reading AttributeDefinition object");
            Assertion.Assert(reader.TokenType == JsonTokenType.EndObject, "AttributeDefinition should end with a end object");
            return new AttributeDefinition(attr_def_name, min_val_count, max_val_count, attr_val_type_name);
        }
        static public void WriteAttributeDefinition(
                Utf8JsonWriter writer,
                AttributeDefinition attr_def,
                JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("Name", attr_def.Name.AsString());
            writer.WriteNumber("MinValueCount", attr_def.MinValueCount);
            writer.WriteNumber("MaxValueCount", attr_def.MaxValueCount);
            writer.WriteString("ValueTypeName", attr_def.ValueTypeName.AsString());

            writer.WriteEndObject();
        }
        static public void WriteTypeDefinition(
                Utf8JsonWriter writer,
                TypeDefinition type_def,
                JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteEndObject();
        }

        public class HashStringConverter : JsonConverter<HashString>
        {
            public override HashString Read(ref Utf8JsonReader reader,
                                            Type type,
                                            JsonSerializerOptions options)
            {
                return JSonExportUtils.ReadHashString(ref reader, options);
            }

            public override void Write(
                Utf8JsonWriter writer,
                HashString hashstring,
                JsonSerializerOptions options)
            {
                JSonExportUtils.WriteHashString(writer, hashstring, options);
            }
        }
        public class AttributeDefinitionConverter : JsonConverter<AttributeDefinition>
        {
            public override AttributeDefinition Read(ref Utf8JsonReader reader,
                                            Type type,
                                            JsonSerializerOptions options)
            {
                return JSonExportUtils.ReadAttributeDefinition(ref reader, options);
            }

            public override void Write(
                Utf8JsonWriter writer,
                AttributeDefinition attr_def,
                JsonSerializerOptions options)
            {
                JSonExportUtils.WriteAttributeDefinition(writer, attr_def, options);
            }
        }

        public class DataMapConverter : JsonConverter<DataMap>
        {
            public override DataMap Read(ref Utf8JsonReader reader,
                                            Type type,
                                            JsonSerializerOptions options)
            {
                string str = "";
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonTokenType.String:
                            str = reader.GetString();
                            break;
                        default:
                            break;

                    }
                }
                return new DataMap();
            }

            public override void Write(
                Utf8JsonWriter writer,
                DataMap dm,
                JsonSerializerOptions options)
            {
                //dm
                {
                    writer.WriteStartObject();

                    //attr def list
                    {
                        writer.WriteStartArray();

                        //attr def
                        {
                            foreach (AttributeDefinition attr_def in dm.AttributeDefinitions)
                            {
                                JSonExportUtils.WriteAttributeDefinition(writer, attr_def, options);
                            }
                        }

                        writer.WriteEndArray();
                    }
                    //type def list
                    {
                        writer.WriteStartArray();
                        //type def
                        {
                            foreach (TypeDefinition type_def in dm.TypeDefinitions)
                            {
                                writer.WriteStartObject();

                                writer.WriteString("Name", type_def.Name.AsString());

                                writer.WriteEndObject();
                            }
                        }
                        writer.WriteEndArray();
                    }

                    writer.WriteEndObject();
                }
            }
        }
        public static JsonSerializerOptions GetSerializationOptions()
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                IncludeFields = true,
            };
            options.Converters.Add(new HashStringConverter());
            options.Converters.Add(new AttributeDefinitionConverter());

            return options;
        }
    }

    public class JSonExporter : IExporter
    {
        private static JsonSerializerOptions GetSerializationOptions()
        {
            return JSonExportUtils.GetSerializationOptions();
        }

        private string Export(in List<AttributeInstanceDescription> attributes)
        {
            string retval = "";
            foreach (AttributeInstanceDescription attr_desc in attributes)
            {
                retval += JsonSerializer.Serialize<AttributeInstanceDescription>(attr_desc, GetSerializationOptions());
            }

            return retval;
        }

        private string Export(in TypeInstanceDescription type_desc)
        {
            string retval = JsonSerializer.Serialize<TypeInstanceDescription>(type_desc, GetSerializationOptions());
            return retval;
        }
        public void Export(in ExportSettings settings, in List<TypeInstanceDescription> type_desc_list)
        {
            string export_val = "";

            foreach (TypeInstanceDescription type_desc in type_desc_list)
            {
                export_val += Export(type_desc) + "\n";
            }

            Console.WriteLine(export_val);
        }

        public string Export(in DataMap dm)
        {
            string export_val = JsonSerializer.Serialize<DataMap>(dm, GetSerializationOptions());
            return export_val;
        }

        public void Export(in ExportSettings settings, in DataMap dm)
        {
            Console.WriteLine(Export(dm));
        }
    }
}
