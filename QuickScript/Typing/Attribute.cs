using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Typing
{
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
}
