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

            private static string CreateErrorMessage(Reason reason, AttributeInstanceDescription attr_desc)
            {
                switch (reason)
                {
                    case Reason.CouldNotFindDefinition:
                        return "Could not find definition for attribute " + attr_desc.Name.ToString() + ". Maybe it is misspelt or the attribute was not parsed originally!";

                    case Reason.ValueCountIsLessThanMinimum:
                        return "Attribute " + attr_desc.Name.ToString() + " does not have enough value arguments!";

                    case Reason.ValueCountIsGreaterThanMaximum:
                        return "Attribute " + attr_desc.Name.ToString() + " does not have enough value arguments!";

                    case Reason.ValueTypeIsIncorrect:
                        return "Attribute " + attr_desc.Name.ToString() + " has values that are incompatible with it!";
                }

                return "Could not parse attribute " + attr_desc.Name.ToString() + " with unknown reason";
            }

            public InvalidAttributeDescription(Reason reason, AttributeInstanceDescription attr_desc) : base(CreateErrorMessage(reason, attr_desc))
            {

            }
        }
        public static AttributeTag ParseAttributeDescriptionIntoTag(AttributeDefinition attr_def, AttributeInstanceDescription attr_desc)
        {
            List<ValueType> attr_values = null;
            int value_count = attr_desc.HasValues() ? attr_desc.Values.Count : 0;

            if (attr_def.MinValueCount >= 0 && value_count < attr_def.MinValueCount)
            {
                throw new InvalidAttributeDescription(InvalidAttributeDescription.Reason.ValueCountIsLessThanMinimum, attr_desc);
            }
            if (attr_def.MaxValueCount >= 0 && value_count > attr_def.MaxValueCount)
            {
                throw new InvalidAttributeDescription(InvalidAttributeDescription.Reason.ValueCountIsGreaterThanMaximum, attr_desc);
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
                throw new InvalidAttributeDescription(InvalidAttributeDescription.Reason.CouldNotFindDefinition, attr_desc);
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
                    AttributeTag new_attr = TypeInformationUtils.ParseAttributeDescriptionIntoTag(dm, attr_desc);
                    retval.Add(new_attr);
                }

                return retval;
            }
            catch
            {
                throw;
            }
        }

        public class InvalidTypeDescription : Exception
        {
            public enum Reason
            {
                Unknown,
                ExistingDefinition,
                MemberTypeNotFound,
                MembersWithSameName,
                MemberHasInvalidAttributes,
                InvalidAttributes
            };

            public Reason FailReason = Reason.Unknown;

            public InvalidTypeDescription(Reason reason, TypeInstanceDescription parent) : base() 
            {
                FailReason = reason;
            }

            public InvalidTypeDescription(Reason reason, string message) : base(message)
            {
                FailReason = reason;
            }
        }

        public static List<TypeDefinition.MemberDefinition> ParseMemberDescriptionsIntoDefinitions(DataMap dm, List<TypeInstanceDescription.MemberDescription> mem_descriptions, TypeInstanceDescription parent) 
        {
            Assertion.Assert(mem_descriptions.Count > 0, "Should only call this method when we have attr descriptions");
            try
            {
                List<TypeDefinition.MemberDefinition> retval = new List<TypeDefinition.MemberDefinition>();

                foreach (TypeInstanceDescription.MemberDescription mem_desc in mem_descriptions)
                {
                    TypeDefinition? member_type = dm.GetTypeDefinitionByName(mem_desc.TypeName);
                    if (member_type == null)
                    {
                        throw new InvalidTypeDescription(InvalidTypeDescription.Reason.MemberTypeNotFound, "Member " + mem_desc.Name.ToString() + " has type " + mem_desc.TypeName.ToString() + " that could not be found!");
                    }

                    if (mem_descriptions.Find(x => x != mem_desc && x.Name == mem_desc.Name) != null)
                    {
                        throw new InvalidTypeDescription(InvalidTypeDescription.Reason.MembersWithSameName, "Member " + mem_desc.Name.ToString() + " has same name as another member! Cannot have two members with the same name!");
                    }

                    TypeDefinition.MemberDefinition new_member = new TypeDefinition.MemberDefinition
                    {
                        Name = mem_desc.Name,
                        Type = member_type
                    };

                    if (mem_desc.HasValue())
                    {
                        new_member.Value = mem_desc.Value;
                    }

                    if (mem_desc.HasAttributes())
                    {
                        try
                        {
                            new_member.Attributes = ParseAttributeDescriptionsIntoTags(dm, mem_desc.Attributes);
                        }
                        catch (InvalidAttributeDescription e)
                        {
                            throw;
                        }
                    }
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
