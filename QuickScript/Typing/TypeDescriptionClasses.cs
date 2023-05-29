﻿using System.Linq;
using System.Security.Cryptography.X509Certificates;
using QuickScript.Utils;

namespace QuickScript.Typing
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
            return Val == ((ValueType)o).Val;
        }
        public bool IsEmpty()
        {
            return Val == "";
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

        public string GetName() { return Name.AsString(); }
    }

    public class AttributeTag
    {
        public HashString AttributeName { get; set; }
        public List<ValueType>? Values { get; set; }
        public bool HasValues() { return Values != null && Values.Count > 0; }

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
                if (x.Values.SequenceEqual(y.Values) == false)
                {
                    return false;
                }
            }

            return true;
        }
    }

    public class AttributeInstanceDescription
    {
        public HashString Name { get; set; }
        public List<ValueType>? Values { get; set; }

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
        public class MethodDefinition
        {
            public class MethodArgument
            {
                public HashString Name;
                public HashString TypeName;
                public MethodArgument(HashString name, HashString type)
                {
                    Name = name;
                    TypeName = type;
                }
                public override int GetHashCode()
                {
                    return Name.GetHashCode() ^ TypeName.GetHashCode();
                }
                public override bool Equals(object o)
                {
                    if (!(o is MethodArgument))
                        return false;

                    MethodArgument x = (MethodArgument)o;
                    MethodArgument y = this;

                    if (x.Name != y.Name ||
                        x.TypeName != y.TypeName)
                    {
                        return false;
                    }
                    return true;
                }
            }
            public HashString Name { get; set; }
            public List<AttributeTag>? Attributes { get; set; }
            public HashString ReturnTypeDef = new HashString("void");
            public List<MethodArgument>? Arguments;
            public bool HasArguments() { return Arguments != null && Arguments.Count > 0; }
            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ ReturnTypeDef.GetHashCode();
            }
            public override bool Equals(object o)
            {
                if (!(o is MethodDefinition))
                    return false;

                MethodDefinition x = (MethodDefinition)o;
                MethodDefinition y = this;

                if (x.Name != y.Name ||
                    x.ReturnTypeDef != y.ReturnTypeDef)
                {
                    return false;
                }
                return true;
            }
        }
        public class MemberDefinition
        {
            public HashString Name { get; set; } = new HashString();
            public List<AttributeTag> Attributes { get; set; } = new List<AttributeTag>();
            public HashString TypeName { get; set; }
            public ValueType Value { get; set; } = new ValueType();
            public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }
            public bool HasValue() { return Value != null && Value != ""; }
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
                    x.Attributes.SequenceEqual(y.Attributes);
                    if (x.Attributes.SequenceEqual(y.Attributes) == false)
                        return false;
                }

                if (x.HasValue() && x.Value.Equals(y.Value))
                {
                    return false;
                }

                return true;
            }
        }
        public HashString Name { get; set; }
        public List<MethodDefinition>? Methods { get; set; }
        public bool HasMethods() { return Methods != null && Methods.Count > 0; }
        public List<MemberDefinition>? Members { get; set; }
        public bool HasMembers() { return Members != null && Members.Count > 0; }
        public List<AttributeTag>? Attributes { get; set; }
        public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }
        public AttributeTag FindAttributeByName(HashString name)
        {
            if (HasAttributes())
            {
                return Attributes.Find(x => x.AttributeName == name);
            }
            return null;
        }
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
        public string GetName() { return Name.AsString(); }
    }

    public class TypeInstanceDescription
    {
        public HashString Name { get; set; } = new HashString();
        public List<AttributeInstanceDescription>? Attributes { get; set; }
        public ValueType? Value { get; set; }
        public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }
        public class MethodDescription
        {
            public HashString Name { get; set; } = new HashString("");
            public HashString ReturnTypeDef { get; set; } = new HashString("void");
            public List<AttributeInstanceDescription>? Attributes { get; set; }
        }
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

        public bool HasMembers() { return Members != null && Members.Count > 0; }
    }
}