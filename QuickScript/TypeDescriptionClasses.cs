using QuickScript.Utils;

namespace QuickScript
{
    public class AttributeDefinition
    {        
        public HashString Name { get; set; }
        public uint MinValueCount { get; set; } = 0;
        public uint MaxValueCount { get; set; } = 0;
        public AttributeDefinition(HashString name, uint minValueCount, uint maxValueCount)
        {
            Name = name;
            MinValueCount = minValueCount;
            MaxValueCount = maxValueCount;
        }

        public AttributeDefinition(HashString name) 
        {
            Name = name;
        }
    }

    public class AttributeTag
    {
        public AttributeDefinition Attribute { get; set; }
        public List<string>? Values { get; set; }
    }

    public class AttributeInstanceDescription
    {
        public HashString Name { get; set; }
        public List<string> Values { get; set;}
    }

    public class TypeDefinition
    {
        public class MemberDefinition
        {
            public HashString Name { get; set;} = new HashString();
            public List<AttributeTag> Attributes { get; set; } = new List<AttributeTag>();
            public TypeDefinition Type { get; set; }
            public string DefaultValue { get; set; } = "";

        }
        public HashString Name { get; set; }
        public List<MemberDefinition>? Members { get; set; }
        public string DefaultValue { get; set; }
    }

    public class TypeInstanceDescription
    {
        public HashString Name { get; set;}
        public List<AttributeInstanceDescription> Attributes { get; set; }
        public string Value { get; set; }

        public class MemberDescription
        {
            HashString Name { get; set; }
            string Value { get; set; }
            TypeInstanceDescription TypeDescription { get; set; }
        }
        public List<MemberDescription> Members { get; set; }
    }
}