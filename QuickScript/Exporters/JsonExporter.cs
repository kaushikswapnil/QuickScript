using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuickScript.Exporters
{
    public class JSonExporter : IExporter
    {
        public static JsonSerializerOptions GetSerializationOptions()
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true,
                IncludeFields = true,
            };
            options.Converters.Add(new HashStringConverter());

            return options;
        }

        public class HashStringConverter : JsonConverter<HashString>
        {
            public override HashString Read(ref Utf8JsonReader reader,
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
                return new HashString(str);
            }

            public override void Write(
                Utf8JsonWriter writer,
                HashString hashstring,
                JsonSerializerOptions options)
            {
                writer.WriteStartObject();
                writer.WriteString("Str", hashstring.AsString());
                writer.WriteEndObject();
            }
        }

        private string Export(in List<AttributeInstanceDescription> attributes)
        {
            string retval = "";
            foreach (AttributeInstanceDescription attr_desc in attributes)
            {
                retval += JsonSerializer.Serialize(attr_desc, GetSerializationOptions());
            }

            return retval;
        }

        private string Export(in TypeInstanceDescription type_desc)
        {
            string retval = JsonSerializer.Serialize(type_desc, GetSerializationOptions());
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
            string export_val = JsonSerializer.Serialize(dm, GetSerializationOptions());
            return export_val;
        }

        public void Export(in ExportSettings settings, in DataMap dm)
        {
            Console.WriteLine(Export(dm));
        }
    }
}
