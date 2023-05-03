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
        public List<string>? Values { get; set;}

        public AttributeInstanceDescription(HashString name)
        {
            Name = name;
            Values = null;
        }
        public AttributeInstanceDescription(HashString name, List<string> values)
        {
            Name = name;
            Values = values;
        }
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
        public List<AttributeTag>? Attributes { get; set; }
        public string? DefaultValue { get; set; }
    }

    public class TypeInstanceDescription
    {
        public HashString Name { get; set; } = new HashString();
        public List<AttributeInstanceDescription>? Attributes { get; set; }
        public string? Value { get; set; }
        public bool HasAttributes() { return Attributes != null && Attributes.Count > 0;}
        public class MemberDescription
        {
            public HashString Name { get; set; } = new HashString();
            public string? Value { get; set; }
            public TypeInstanceDescription TypeDescription { get; set; } = new TypeInstanceDescription();
            public List<AttributeInstanceDescription>? Attributes { get; set; }

            public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }
            public bool HasValue() { return Value != null; }
        }
        public List<MemberDescription>? Members { get; set; }

        public bool HasMembers() { return Members != null && Members.Count > 0;}
    }
}