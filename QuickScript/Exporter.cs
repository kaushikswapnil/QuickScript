using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript
{
    public class Exporter
    {
        public virtual string Export(in List<AttributeInstanceDescription> attributes)
        {
            string retval = "";

            retval += "[";
            foreach (AttributeInstanceDescription attr_desc in attributes)
            {
                retval += attr_desc.Name.AsString() + ", ";
            }
            retval += "]";

            return retval;
        }

        public virtual string Export(in TypeInstanceDescription type_desc)
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

        public virtual string Export(in List<TypeInstanceDescription> type_desc_list)
        {
            string retval = "";

            foreach (TypeInstanceDescription type_desc in type_desc_list)
            {
                retval += Export(type_desc) + "\n";
            }

            return retval;
        }
    }
}
