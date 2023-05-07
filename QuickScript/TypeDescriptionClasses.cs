using QuickScript.Utils;
using System.Security.Cryptography.X509Certificates;

namespace QuickScript
{
    public class ValueType
    {
        public string Val;
        public ValueType() 
        {
            Val = "";
        }
        public ValueType(string val)
        {
            Val = val;
        }

        public static implicit operator string(ValueType val) => val.Val;
        public static explicit operator ValueType(string val) => new ValueType(val);

        public string AsString() { return Val; }
        public override int GetHashCode()
        {
            return Val.GetHashCode();
        }
        public override bool Equals(object o)
        {
            if (!(o is ValueType))
                return false;
            return this.Val == ((ValueType)o).Val;
        }
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

        public override int GetHashCode()
        {
            return Name.GetHashCode() ^ MinValueCount.GetHashCode() ^ MaxValueCount.GetHashCode(); 
        }

        public override bool Equals(object o)
        {
            if (!(o is AttributeDefinition))
                return false;

            AttributeDefinition x = this;
            AttributeDefinition y = (AttributeDefinition)o;
            return x.Name == y.Name && x.MinValueCount == y.MinValueCount && x.MaxValueCount == y.MaxValueCount && x.ValueTypeName == y.ValueTypeName;
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
        public AttributeTag(HashString attr_name, in List<ValueType> values)
        {
            AttributeName = attr_name;
            Values = values;
        }
        public override int GetHashCode()
        {
            int retval = AttributeName.GetHashCode();
            return retval;
        }
        public override bool Equals(object o)
        {
            if (!(o is AttributeTag))
                return false;
            AttributeTag y = (AttributeTag)o;
            AttributeTag x = this;
            if (x.AttributeName != y.AttributeName ||
                    x.HasValues() != y.HasValues())
            {
                return false;
            }

            if (x.HasValues())
            {
                for (int tag_val_iter = 0; tag_val_iter < x.Values.Count; ++tag_val_iter)
                {
                    if (x.Values[tag_val_iter] != y.Values[tag_val_iter])
                        return false;
                }
            }

            return true;
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
            public ValueType Value { get; set; } = new ValueType();
            public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }
            public bool HasValue() { return Value != null; }
            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ TypeName.GetHashCode() ^ Value.GetHashCode();
            }
            public override bool Equals(object o)
            {
                if (!(o is MemberDefinition))
                    return false;

                MemberDefinition x = (MemberDefinition)o;
                MemberDefinition y = this;

                if (x.Name != y.Name ||
                    x.TypeName != y.TypeName ||
                    x.HasAttributes() != y.HasAttributes() ||
                    x.HasValue() != y.HasValue())
                {
                    return false;
                }

                if (x.HasAttributes())
                {
                    for (int attr_tag_iter = 0; attr_tag_iter < x.Attributes.Count; ++attr_tag_iter)
                    {
                        if (x.Attributes[attr_tag_iter] != y.Attributes[attr_tag_iter])
                            return false;
                    }
                }

                if (x.HasValue() && x.Value != y.Value)
                {
                    return false;
                }

                return true;
            }
        }
        public HashString Name { get; set; }
        public List<MemberDefinition>? Members { get; set; }
        public bool HasMembers() { return Members != null && Members.Count > 0; }
        public List<AttributeTag>? Attributes { get; set; }
        public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }

        public TypeDefinition()
        {
            Name = new HashString();
            Members = null;
            Attributes = null;
        }

        public TypeDefinition(HashString name)
        {
            Name = name;
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        public override bool Equals(object o)
        {
            if (!(o is TypeDefinition))
                return false;

            TypeDefinition x = (TypeDefinition)o;
            TypeDefinition y = this;

            if (x.Name != y.Name ||
                x.HasMembers() != y.HasMembers() ||
                x.HasAttributes() != y.HasAttributes())
            {
                return false;
            }
            if (x.HasAttributes())
            {
                if (x.Attributes.SequenceEqual(y.Attributes) == false)
                {
                    return false;
                }
            }
            if (x.HasMembers())
            {
                if (x.Members.SequenceEqual(y.Members) == false)
                {
                    return false;
                }
            }

            return true;
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