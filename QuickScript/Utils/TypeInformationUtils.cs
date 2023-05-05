using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Utils
{
    public static class TypeInformationUtils
    {
        public class InvalidAttributeDescription : Exception
        {
            public enum Reason
            {
                Unknown,
                CouldNotFindDefinition,
                ValueCountIsLessThanMinimum,
                ValueCountIsGreaterThanMaximum,
                ValueTypeIsIncorrect
            };

            public Reason FailReason = Reason.Unknown;

            public InvalidAttributeDescription(Reason reason)
            {
                FailReason = reason;
            }
        }
        public static AttributeTag ParseAttributeDescriptionIntoTag(AttributeDefinition attr_def, AttributeInstanceDescription attr_desc)
        {
            List<ValueType> attr_values = null;
            int value_count = attr_desc.HasValues() ? attr_desc.Values.Count : 0;

            if (attr_def.MinValueCount >= 0 && value_count < attr_def.MinValueCount)
            {
                throw new InvalidAttributeDescription(InvalidAttributeDescription.Reason.ValueCountIsLessThanMinimum);
            }
            if (attr_def.MaxValueCount >= 0 && value_count > attr_def.MaxValueCount)
            {
                throw new InvalidAttributeDescription(InvalidAttributeDescription.Reason.ValueCountIsGreaterThanMaximum);
            }

            //#TODO test value types
            if (attr_desc.HasValues())
            {
                attr_values = attr_desc.Values;
            }

            return new AttributeTag(attr_def, attr_values);
        }

        public static AttributeTag ParseAttributeDescriptionIntoTag(DataMap dm, AttributeInstanceDescription attr_desc)
        {
            AttributeDefinition? attr_def = dm.GetAttributeDefinitionByName(attr_desc.Name);
            if (attr_def == null)
            {
                throw new InvalidAttributeDescription(InvalidAttributeDescription.Reason.CouldNotFindDefinition);
            }

            try
            {
                return ParseAttributeDescriptionIntoTag(attr_def, attr_desc);
            }
            catch
            {
                throw;
            }
        }

        public static List<AttributeTag> ParseAttributeDescriptionsIntoTags(DataMap dm, List<AttributeInstanceDescription> attr_descriptions)
        {
            Assertion.Assert(attr_descriptions.Count > 0, "Should only call this method when we have attr descriptions");
            try
            {
                List<AttributeTag> retval = new List<AttributeTag>();

                foreach (AttributeInstanceDescription attr_desc in attr_descriptions)
                {
                    AttributeTag new_attr = TypeInformationUtils.ParseAttributeDescriptionIntoTag(this, attr_desc);
                    retval.Add(new_attr);
                }

                return retval;
            }
            catch
            {
                throw;
            }
        }
    }
}
