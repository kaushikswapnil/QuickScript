using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript
{
    public interface IExporter
    {
        public void Export(in List<TypeInstanceDescription> type_desc_list);
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

        public void Export(in List<TypeInstanceDescription> type_desc_list)
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

