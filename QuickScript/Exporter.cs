using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using QuickScript.Utils;
using System.Globalization;
using System.Text.Json.Nodes;

namespace QuickScript
{
    public class ExportSettings
    {

    }

    public interface IExporter
    {
        public void Export(in ExportSettings settings, in List<TypeInstanceDescription> type_desc_list);
    }

    public class ConsoleExporter : IExporter
    {
        private string Export(in List<AttributeInstanceDescription> attributes)
        {
            string retval = "";

            retval += "[";
            foreach (AttributeInstanceDescription attr_desc in attributes)
            {
                retval += attr_desc.Name.AsString();
                if (attr_desc.Values != null)
                {
                    retval += "(";
                    foreach (var attr_desc_val in attr_desc.Values)
                    {
                        retval += attr_desc_val + ", ";
                    }
                    retval += ")";
                }
                retval += ", ";
            }
            retval += "]";

            return retval;
        }

        private string Export(in TypeInstanceDescription type_desc)
        {
            string retval = "";

            if (type_desc.HasAttributes())
            {
                retval += Export(type_desc.Attributes) + "\n";
            }
            retval += type_desc.Name.AsString() + "\n";
            retval += "{\n";

            if (type_desc.HasMembers())
            {
                foreach (TypeInstanceDescription.MemberDescription member in type_desc.Members)
                {
                    retval += member.TypeDescription.Name.AsString() + " " + member.Name.AsString();
                    if (member.HasValue())
                    {
                        retval += " = " + member.Value;
                    }
                    retval += ";\n";
                }
            }

            retval += "}";

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
    }

    public class JSonExporter : IExporter
    {
        private static JsonSerializerOptions GetSerializationOptions()
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
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
    }
}

