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
using QuickScript.Typing;

namespace QuickScript.Exporters
{
    public class ExportSettings
    {
        public string InputDirectory = QuickscriptSettings.DEFAULT_INPUT_TEST_DIRECTORY;
        public string OutputDirectory = QuickscriptSettings.DEFAULT_OUTPUT_TEST_DIRECTORY;
        public bool DebugLog = true;

        [Flags]
        public enum ESettingFlags : uint
        {
            Default = 0,
            WriteToConsole = 1 << 0,
            WriteToOutputDirectory = 1 << 1,
        }
        public ESettingFlags SettingFlags = (ESettingFlags.WriteToOutputDirectory) | (ESettingFlags.WriteToConsole);
    }

    public abstract class IExporter
    {
        public abstract void Export(in ExportSettings settings, in List<TypeInstanceDescription> type_desc_list);
        public abstract void Export(in ExportSettings settings, in DataMap dm);
        public static void DebugLog(in ExportSettings settings, string message)
        {
            if (settings.DebugLog)
            {
                Logging.Log(message);
            }
        }
    }

    public class ConsoleExporter : IExporter
    {
        private string Export(in List<AttributeInstanceDescription> attributes)
        {
            string retval = "";

            retval += "[";

            void AddAttrDesc(AttributeInstanceDescription attr)
            {
                retval += attr.Name.AsString();
                if (attr.Values != null && attr.Values.Count > 0)
                {
                    retval += "(";
                    var attr_val = attr.Values[0];
                    for (int i = 1; i < attr.Values.Count; i++)
                    {
                        attr_val = attr.Values[i];
                        retval += ", " + attr_val;
                    }

                    retval += ")";
                }
            }

            if (attributes.Count > 0)
            {
                AddAttrDesc(attributes[0]);
                for (int i = 1; i < attributes.Count; ++i)
                {
                    retval += ", ";
                    AddAttrDesc(attributes[i]);
                }
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
                    retval += member.TypeName.AsString() + " " + member.Name.AsString();
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

        public override void Export(in ExportSettings settings, in List<TypeInstanceDescription> type_desc_list)
        {
            string export_val = "";

            foreach (TypeInstanceDescription type_desc in type_desc_list)
            {
                export_val += Export(type_desc) + "\n";
            }

            Console.WriteLine(export_val);
        }

        private string Export(in List<AttributeDefinition> attr_list)
        {
            string retval = "-----Attribute List-----\n\n";

            foreach (AttributeDefinition attribute in attr_list)
            {
                retval += "{";
                retval += "\nName : " + attribute.Name.AsString();
                retval += "\nValue count : " + attribute.MinValueCount.ToString() + ", " + attribute.MaxValueCount.ToString();
                retval += "\n}\n";
            }

            return retval;
        }

        private string Export(in List<AttributeTag> attributeTags)
        {
            string retval = "[";

            for (int attr_i = 0; attr_i < attributeTags.Count; ++attr_i)
            {
                var attr_tag = attributeTags[attr_i];
                retval += attr_i > 0 ? ", " : "";
                retval += attr_tag.AttributeName.ToString();
                if (attr_tag.HasValues())
                {
                    retval += "(";
                    for (int val_i = 0; val_i < attr_tag.Values.Count; ++val_i)
                    {
                        var val = attr_tag.Values[val_i];
                        retval += val_i > 0 ? ", " : "";
                        retval += val;
                    }
                    retval += ")";
                }
            }

            retval += "]";

            return retval;
        }

        private string Export(in List<TypeDefinition> typeDefinitions)
        {
            string retval = "-----Type Definitions-----\n";

            foreach (TypeDefinition typeDefinition in typeDefinitions)
            {
                if (typeDefinition.HasAttributes())
                {
                    retval += "\n" + Export(typeDefinition.Attributes);
                }
                retval += "\nName : " + typeDefinition.Name.AsString();
                retval += "\n{";

                if (typeDefinition.HasMembers())
                {
                    foreach (TypeDefinition.MemberDefinition member in typeDefinition.Members)
                    {
                        if (member.HasAttributes())
                        {
                            retval += "\n" + Export(member.Attributes);
                        }
                        retval += "\n" + member.TypeName.ToString() + " " + member.Name.ToString();
                        if (member.HasValue())
                        {
                            retval += " = " + member.Value;
                        }
                        retval += ";";
                    }
                }

                retval += "\n}\n";
            }

            return retval;
        }

        public override void Export(in ExportSettings settings, in DataMap dm)
        {
            string export_val = "";
            export_val += Export(dm.AttributeDefinitions);
            export_val += Export(dm.TypeDefinitions);

            Console.WriteLine(export_val);
        }
    }
}

