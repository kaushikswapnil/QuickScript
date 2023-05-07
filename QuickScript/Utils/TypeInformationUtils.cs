using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Utils
{
    public static class TypeInformationUtils
    {
        public static AttributeInstanceDescription? GetAttributeInstanceDescription(HashString attr_name, TypeInstanceDescription type_desc)
        {
            if (type_desc.HasAttributes())
            {
                return type_desc.Attributes.Find(x => x.Name == attr_name);
            }

            return null;
        }

        public static bool HasAttributeDescription(HashString attr_name, TypeInstanceDescription type_desc)
        {
            return GetAttributeInstanceDescription(attr_name, type_desc) != null;
        }

        public static List<ValueType>? GetAttributeDescriptionValues(HashString attr_name, TypeInstanceDescription type_desc)
        {
            var attr_desc = GetAttributeInstanceDescription(attr_name, type_desc);
            if (attr_desc != null)
            {
                return attr_desc.Values;
            }

            return null;
        }

        public static ValueType? GetAttributeDescriptionFirstValue(HashString attr_name, TypeInstanceDescription type_desc)
        {
            List<ValueType> values = GetAttributeDescriptionValues(attr_name, type_desc);
            if (values != null)
            {
                return values[0];
            }

            return null;
        }

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

                Assertion.Assert(reason == Reason.Unknown, "Unhandled reason in InvalidAttributeDescription");

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

            static string CreateTypeNameFileNameCombination(TypeInstanceDescription parent)
            {
                string retval = parent.Name.AsString();
                ValueType? parent_file_path = GetAttributeDescriptionFirstValue(new HashString("FilePath"), parent);
                if (parent_file_path != null)
                {
                    retval += "[" + parent_file_path.AsString() + "]";
                }

                return retval;
            }

            static string CreateErrorMessage(Reason reason, TypeInstanceDescription parent, TypeInstanceDescription.MemberDescription fail_member, InvalidAttributeDescription ex)
            {
                string type_file_name_combo = CreateTypeNameFileNameCombination(parent);
                switch (reason)
                {
                   case Reason.MemberTypeNotFound:
                        Assertion.Assert(fail_member != null, "Should have a member that failed here!");
                        return "Type " + type_file_name_combo + " could not be parsed because member " + fail_member.Name.AsString() + 
                            " had a type " + fail_member.TypeName.AsString() + " that could not be found";

                   case Reason.MembersWithSameName:
                        Assertion.Assert(fail_member != null, "Should have a member that failed here");
                        return "Type " + type_file_name_combo + " could not be parsed because member " + fail_member.Name.AsString() +
                            " has same name as another member";
                   case Reason.MemberHasInvalidAttributes:
                        Assertion.Assert(fail_member != null, "Should have a member that failed here!");
                        Assertion.Assert(ex != null, "Should have an invalid attr ex if we are here");
                        return "Type " + type_file_name_combo + " could not be parsed because member " + fail_member.Name.AsString() +
                            " has invalid attributes. Error: " + ex.Message;

                   case Reason.InvalidAttributes:
                        Assertion.Assert(ex != null, "Should have an invalid attr ex if we are here");
                        return "Type " + type_file_name_combo + " has invalid attributes and could not be handled! Error: " + ex.Message;

                }

                Assertion.Assert(reason == Reason.Unknown, "Unhandled reason in InvalidTypeDescription");

                return "Could not handle type " + type_file_name_combo;
            }

            public InvalidTypeDescription(Reason reason, TypeInstanceDescription parent,
                TypeInstanceDescription.MemberDescription fail_member, InvalidAttributeDescription ex) 
                : base(CreateErrorMessage(reason, parent, fail_member, ex))
            {
            }
        }

        public static List<TypeDefinition.MemberDefinition> ParseMemberDescriptionsIntoDefinitions(DataMap dm, TypeInstanceDescription parent) 
        {
            List<TypeInstanceDescription.MemberDescription> mem_descriptions = parent.Members;
            Assertion.Assert(mem_descriptions.Count > 0, "Should only call this method when we have attr descriptions");
            try
            {
                List<TypeDefinition.MemberDefinition> retval = new List<TypeDefinition.MemberDefinition>();

                foreach (TypeInstanceDescription.MemberDescription mem_desc in mem_descriptions)
                {
                    TypeDefinition? member_type = dm.GetTypeDefinitionByName(mem_desc.TypeName);
                    if (member_type == null)
                    {
                        throw new InvalidTypeDescription(InvalidTypeDescription.Reason.MemberTypeNotFound, parent, mem_desc, null);
                    }

                    if (mem_descriptions.Find(x => x != mem_desc && x.Name == mem_desc.Name) != null)
                    {
                        throw new InvalidTypeDescription(InvalidTypeDescription.Reason.MembersWithSameName, parent, mem_desc, null);
                    }

                    TypeDefinition.MemberDefinition new_member = new TypeDefinition.MemberDefinition
                    {
                        Name = mem_desc.Name,
                        TypeName = mem_desc.TypeName
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
                            throw new InvalidTypeDescription(InvalidTypeDescription.Reason.MemberHasInvalidAttributes, parent, mem_desc, e);
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

        static public TypeDefinition ParseTypeDescriptionToPotentialDefinition(DataMap dm, TypeInstanceDescription description)
        {
            List<AttributeTag> attr_tags = null;
            List<TypeDefinition.MemberDefinition> members = null;
            if (description.HasAttributes())
            {
                //first try to create attribute tags for each attr inst
                attr_tags = TypeInformationUtils.ParseAttributeDescriptionsIntoTags(dm, description.Attributes);
            }

            if (description.HasMembers())
            {
                members = TypeInformationUtils.ParseMemberDescriptionsIntoDefinitions(dm, description);
            }

            TypeDefinition new_def = new TypeDefinition(description.Name);
            new_def.Attributes = attr_tags;
            new_def.Members = members;

            return new_def;
        }
        public static List<AttributeDefinition> SupportedAttributeDefinitionList()
        {
            List<AttributeDefinition> retval = new List<AttributeDefinition>();

            {
                //Filepath
                AttributeDefinition file_path = new AttributeDefinition(new HashString("FilePath"), 1, new HashString("string"));
                retval.Add(file_path);
            }
            {
                //Alias
                AttributeDefinition alias = new AttributeDefinition(new HashString("Alias"), 1, -1, new HashString("string"));
                retval.Add(alias);
            }
            {
                //DefaultValue
                AttributeDefinition def_val = new AttributeDefinition(new HashString("DefaultValue"), 0, 1, new HashString("string"));
                retval.Add(def_val);
            }
            {
                //Precision
                AttributeDefinition def_val = new AttributeDefinition(new HashString("Precision"), 0, 1, new HashString("int"));
                retval.Add(def_val);
            }

            return retval;
        }
        public static List<TypeDefinition> SupportedBasicTypeDefinitionsList()
        {
            List<TypeDefinition> retval = new List<TypeDefinition>();
            {
                //bool
                TypeDefinition boolean = new TypeDefinition
                {
                    Name = new HashString("bool"),
                    Attributes = new List<AttributeTag> { new AttributeTag(new HashString("DefaultValue"), new List<ValueType> { new ValueType("true") }) }
                };
                retval.Add(boolean);
            }

            {
                //int
                TypeDefinition integer = new TypeDefinition
                {
                    Name = new HashString("int"),
                    Attributes = new List<AttributeTag> { new AttributeTag(new HashString("DefaultValue"), new List<ValueType> { new ValueType("0") }) }
                };
                retval.Add(integer);
            }

            {
                //char
                TypeDefinition character = new TypeDefinition
                {
                    Name = new HashString("char"),
                    Attributes = new List<AttributeTag> { new AttributeTag(new HashString("DefaultValue"), new List<ValueType> { new ValueType(" ") }) }
                };
                retval.Add(character);
            }

            {
                //float
                TypeDefinition floating = new TypeDefinition
                {
                    Name = new HashString("float"),
                    Attributes = new List<AttributeTag> { new AttributeTag(new HashString("DefaultValue"), new List<ValueType> { new ValueType("0.0") }) }
                };
                retval.Add(floating);
            }

            {
                //string
                TypeDefinition str = new TypeDefinition
                {
                    Name = new HashString("string"),
                    Attributes = new List<AttributeTag> { new AttributeTag(new HashString("DefaultValue"), new List<ValueType> { new ValueType("") }) }
                };
                retval.Add(str);
            }
            return retval;
        }
    }
}
