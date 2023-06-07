using System.Linq;
using System.Security.Cryptography.X509Certificates;
using QuickScript.Utils;
using static QuickScript.Utils.HashString;

namespace QuickScript.Typing
{
    public class TypeDefinition
    {
        public class MemberDefinition
        {
            public HashString Name { get; set; } = new HashString();
            public List<AttributeTag> Attributes { get; set; } = new List<AttributeTag>();
            public HashString TypeName { get; set; }
            public ValueType Value { get; set; } = new ValueType();
            public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }
            public bool HasValue() { return Value != ""; }
            public override int GetHashCode()
            {
                return Name.GetHashCode() ^ TypeName.GetHashCode();
            }
            public override bool Equals(object o)
            {
                if (!(o is MemberDefinition))
                    return false;

                MemberDefinition x = (MemberDefinition)o;
                MemberDefinition y = this;

                if (x.Name != y.Name ||
                    x.TypeName != y.TypeName)
                {
                    return false;
                }

                return true;
            }
        }

        public class MethodDefinition
        {
            public List<AttributeTag>? Attributes { get; set; }
            public List<MemberDefinition> Arguments { get; set; } = new List<MemberDefinition>();
            public HashString Name { get; set; }
            public HashString ReturnTypeDef { get; set; }
            public bool HasArguments() { return Arguments.Count > 0; }
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

                if (x.Name.Equals(y.Name) == false ||
                    x.ReturnTypeDef.Equals(y.ReturnTypeDef) == false ||
                    y.HasArguments() != x.HasArguments()
                    || (x.HasArguments() && x.Arguments.SequenceEqual(y.Arguments) == false))
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
        public abstract class PropertyDescription
        {
            public HashString Name { get; set; } = new HashString();
            public HashString Type { get; set; } = new HashString();
            public List<AttributeInstanceDescription>? Attributes { get; set; }

            public bool HasAttributes() { return Attributes != null && Attributes.Count > 0; }
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
        public class MethodDescription
        {
            public HashString Name { get; set; } = new HashString("");
            public HashString ReturnTypeDef { get; set; } = new HashString("void");
            public List<AttributeInstanceDescription>? Attributes { get; set; }
            //function arguments are just treated as members
            public List<MemberDescription> Arguments { get; set; } = new List<MemberDescription>();
        }
        public List<MemberDescription>? Members { get; set; }

        public bool HasMembers() { return Members != null && Members.Count > 0; }
    }
}