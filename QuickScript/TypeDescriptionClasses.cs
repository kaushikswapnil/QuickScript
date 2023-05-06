using QuickScript.Utils;

namespace QuickScript
{
    public class ValueType
    {
        public string Val;

        public ValueType(string val)
        {
            Val = val;
        }

        public static implicit operator string(ValueType val) => val.Val;
        public static explicit operator ValueType(string val) => new ValueType(val);

        public string AsString() { return Val; }
    }

    public class AttributeDefinition
    {        
        public HashString Name { get; set; }
        public int MinValueCount { get; set; } = 0;
        public int MaxValueCount { get; set; } = 0;
        public HashString ValueTypeName = new HashString();
        public AttributeDefinition(HashString name, int minValueCount, int maxValueCount, HashString valueTypeName)
        {
            Name = name;
            MinValueCount = minValueCount;
            MaxValueCount = maxValueCount;
            ValueTypeName = valueTypeName;
        }

        public AttributeDefinition(HashString name) 
        {
            Name = name;
        }

        public AttributeDefinition(HashString name, int value_count, HashString valueTypeName)
        {
            Name = name;
            MinValueCount = MaxValueCount = value_count;
            ValueTypeName = valueTypeName;
        }
    }

    public class AttributeTag
    {
        public HashString AttributeName { get; set; }
        public List<ValueType>? Values { get; set; }
        public bool HasValues() { return Values != null && Values.Count > 0;}

        public AttributeTag(AttributeDefinition attr_def)
        {
            AttributeName = attr_def.Name;
            Values = null;
        }
        public AttributeTag(AttributeDefinition attr_def, in List<ValueType> values)
        {
            AttributeName = attr_def.Name;
            Values = values;
        }
    }

    public class AttributeInstanceDescription
    {
        public HashString Name { get; set; }
        public List<ValueType>? Values { get; set;}

        public bool HasValues() { return Values != null && Values.Count > 0; }

        public AttributeInstanceDescription(HashString name)
        {
            Name = name;
            Values = null;
        }
        public AttributeInstanceDescription(HashString name, List<ValueType> values)
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
            public HashString TypeName { get; set; }
            public ValueType? Value { get; set; }
            public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }
            public bool HasValue() { return Value != null; }

        }
        public HashString Name { get; set; }
        public List<MemberDefinition>? Members { get; set; }
        public bool HasMembers() { return Members != null && Members.Count > 0; }
        public List<AttributeTag>? Attributes { get; set; }
        public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }
        public ValueType? DefaultValue { get; set; }

        public TypeDefinition(HashString name)
        {
            Name = name;
        }
    }

    public class TypeInstanceDescription
    {
        public HashString Name { get; set; } = new HashString();
        public List<AttributeInstanceDescription>? Attributes { get; set; }
        public ValueType? Value { get; set; }
        public bool HasAttributes() { return Attributes != null && Attributes.Count > 0;}
        public class MemberDescription
        {
            public HashString Name { get; set; } = new HashString();
            public ValueType? Value { get; set; }
            public HashString TypeName { get; set; } = new HashString();
            public List<AttributeInstanceDescription>? Attributes { get; set; }

            public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }
            public bool HasValue() { return Value != null; }
        }
        public List<MemberDescription>? Members { get; set; }

        public bool HasMembers() { return Members != null && Members.Count > 0;}
    }
}