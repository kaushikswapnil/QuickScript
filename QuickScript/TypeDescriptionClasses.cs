using QuickScript.Utils;

namespace QuickScript
{
    public class AttributeDefinition
    {        
        public string Name { get; set; }
        public uint MinValueCount { get; set; }
        public uint MaxValueCount { get; set;}
    }

    public class AttributeTag
    {
        public AttributeDefinition Attribute { get; set; }
        public List<string>? Values { get; set; }
    }

    public class TypeDefinition
    {
        public class MemberDefinition
        {
            public HashString Name { get; set;} = new HashString();
            public List<AttributeTag> Attributes { get; set; } = new List<AttributeTag>();
            public string DefaultValue { get; set; } = "";

        }
        public HashString Name;
        public List<MemberDefinition>? Members;
    }

    public class TypeInstanceDescription
    {
        public string Name { get; set;}
        public List<AttributeTag> Attributes { get; set; }
        public string Value { get; set; }
        public List<TypeInstanceDescription> Members { get; set; }
    }
}