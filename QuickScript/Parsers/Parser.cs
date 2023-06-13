using QuickScript.Typing;
using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Parsers
{
    using ValueType = Typing.ValueType;
    public class Parser
    {
        public static List<TypeInstanceDescription> ParseDirectory(in string directory_name, in bool parse_sub_directories = true)
        {
            Assertion.Assert(Directory.Exists(directory_name), "Directory to parse does not exist!");

            List<TypeInstanceDescription> retval = new List<TypeInstanceDescription>();

            var file_names = Directory.EnumerateFiles(directory_name, "*.qs",
                                                        parse_sub_directories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (string file_name in file_names)
            {
                var parsed_type_instance_descs = ParseLines(File.ReadAllText(file_name));
                foreach (TypeInstanceDescription type_instance_desc in parsed_type_instance_descs)
                {
                    if (type_instance_desc.HasAttributes() == false)
                    {
                        type_instance_desc.Attributes = new List<AttributeInstanceDescription>();
                    }
                    AttributeInstanceDescription file_path_attr = new AttributeInstanceDescription(new HashString("FilePath"));
                    file_path_attr.Values = new List<ValueType>();
                    file_path_attr.Values.Add(new ValueType(file_name));
                    type_instance_desc.Attributes.Add(file_path_attr);
                }

                retval.AddRange(parsed_type_instance_descs);
            }

            return retval;
        }

        private static List<TypeInstanceDescription> ParseLines(in string descLines)
        {
            List<string> tokens = new List<string>();
            string curWord = "";
            bool reading_comment = false;

            void TryAddProperWordToTokens()
            {
                if (curWord.Length > 0)
                {
                    tokens.Add(curWord);
                    curWord = "";
                }
            }

            foreach (char c in descLines)
            {
                switch (c)
                {
                    case ' ':
                    case '\n':
                    case '\r':
                    case '\t':
                    case ',':
                        if (!reading_comment)
                        {
                            TryAddProperWordToTokens();
                        }
                        break;
                    case ';':
                    case '{':
                    case '}':
                    case '[':
                    case ']':
                    case '(':
                    case ')':
                        if (!reading_comment)
                        {
                            TryAddProperWordToTokens();
                            tokens.Add(char.ToString(c));
                        }
                        break;
                    case '*':
                        reading_comment = !reading_comment;
                        break;
                    case '=':
                        break; //we ignore the equal symbol
                    default:
                        if (!reading_comment)
                        {
                            curWord += c;
                        }
                        break;
                }
            }

            TryAddProperWordToTokens();

            return ExtractTypes(tokens);
        }
        private static List<TypeInstanceDescription> ExtractTypes(in List<string> tokens)
        {
            ReadState prevState = ReadState.None;
            ReadState readState = ReadState.Class;

            Stack<string> unhandled_tokens = new Stack<string>();

            List<TypeInstanceDescription> retVal = new List<TypeInstanceDescription>();

            void ChangeReadState(ReadState newState)
            {
                prevState = readState;
                readState = newState;
            }

            TypeInstanceDescription cur_class = new TypeInstanceDescription();
            List<AttributeInstanceDescription> cur_attributes = new List<AttributeInstanceDescription>();
            List<AttributeInstanceDescription> property_attributes = new List<AttributeInstanceDescription>();
            List<TypeInstanceDescription.MemberDescription> cur_members = new List<TypeInstanceDescription.MemberDescription>();
            List<TypeInstanceDescription.MethodDescription> cur_methods = new List<TypeInstanceDescription.MethodDescription>();
            TypeInstanceDescription.MemberDescription cur_member = new TypeInstanceDescription.MemberDescription();
            TypeInstanceDescription.MethodDescription cur_method = new TypeInstanceDescription.MethodDescription();

            foreach (string token in tokens)
            {
                if (token == "[")
                {
                    Assertion.Assert(readState == ReadState.Class || readState == ReadState.Property, "Invalid flow. Read attributes before class or member");
                    ChangeReadState(ReadState.Attributes);
                }
                else if (token == "]")
                {
                    Assertion.Assert(readState == ReadState.Attributes, "Should be reading attributes when we encounter closing brackets");

                    if (cur_attributes.Count > 0)
                    {
                        if (prevState == ReadState.Class)
                        {
                            cur_class.Attributes = cur_attributes;
                        }
                        else
                        {
                            Assertion.Assert(prevState == ReadState.Property, "Should have been reading a member");
                            property_attributes = cur_attributes;
                        }

                        cur_attributes = new List<AttributeInstanceDescription>();
                    }

                    ChangeReadState(prevState);

                }
                else if (token == "{")
                {
                    Assertion.Assert(readState == ReadState.Class);
                    Assertion.Assert(unhandled_tokens.Count > 0, "Should already have at least one token as a class name!");
                    cur_class.Name.Reset(unhandled_tokens.Pop());
                    ChangeReadState(ReadState.Property);
                    Assertion.Assert(unhandled_tokens.Count == 0, "Still have unhandled tokens after reading class name " +
                        cur_class.Name.AsString() + ". We should have no unhandled tokens at this stage of class def");
                }
                else if (token == "}")
                {
                    if (cur_members.Count > 0)
                    {
                        cur_class.Members = cur_members;
                        cur_members = new List<TypeInstanceDescription.MemberDescription>();
                    }

                    retVal.Add(cur_class);
                    cur_class = new TypeInstanceDescription();
                    ChangeReadState(ReadState.Class);
                    Assertion.Assert(unhandled_tokens.Count == 0, "Still have unhandled tokens after encountering closing brackets for class "
                       + retVal[retVal.Count - 1].Name.AsString() + ". At this point we should have read all tokens!");
                }
                else if (token == ";")
                {
                    Assertion.Assert(readState == ReadState.Property, "Should only encounter ; when reading properties such as members or methods");
                    Assertion.Assert(unhandled_tokens.Count >= 2, "Encountered a ';' but not enough tokens before it");

                    void ReadMemberTokens(ref TypeInstanceDescription.MemberDescription mem_desc, ref Stack<string> member_tokens)
                    {
                        //type, name
                        //for a member, we need at least 2 tokens, possibly 3 as type, name, val
                        Assertion.Assert(member_tokens.Count >= 2, "Could not read member while reading class " +
                            cur_class.Name.AsString() + ", because there were not enough tokens. A member should have atleast a type and a name");
                        mem_desc.TypeName.Reset(member_tokens.Pop());
                        mem_desc.Name.Reset(member_tokens.Pop());
                        if (member_tokens.Count > 0)
                        {
                            mem_desc.Value = new ValueType(member_tokens.Pop());
                        }
                    }
                    string top_token = unhandled_tokens.Pop();
                    if (top_token == ")")
                    {
                        //reading a function
                        Assertion.Assert(unhandled_tokens.Count >= 3, "Should have at least the name and return type for function, while reading class "
                            + cur_class.Name.AsString());
                        top_token = unhandled_tokens.Pop();
                        while (top_token != "(")
                        {
                            Assertion.Assert(unhandled_tokens.Count >= 5, "Did not have enough tokens" +
                                " while reading function in  class "
                            + cur_class.Name.AsString());
                            //read values until we reach ','
                            Stack<string> argument_tokens = new Stack<string>();
                            while (top_token != ",")
                            {
                                if (top_token == "(")
                                {
                                    break;
                                }
                                argument_tokens.Push(top_token);
                                top_token = unhandled_tokens.Pop();
                            }
                            ReadMemberTokens(ref cur_member, ref argument_tokens);
                            cur_method.Arguments.Add(cur_member);
                        }

                        cur_method.Attributes = property_attributes;
                        property_attributes = new List<AttributeInstanceDescription>();
                        cur_methods.Add(cur_method);
                        cur_method = new TypeInstanceDescription.MethodDescription();
                    }
                    else
                    {
                        //reading a member
                        Stack<string> member_tokens = new Stack<string>();
                        //for a member, we need at least 2 tokens, possibly 3 as type, name, val
                        Assertion.Assert(unhandled_tokens.Count >= 2, "Did not get enough tokens while reading member "
                            + member_tokens.Peek() + " while reading class " + cur_class.Name.AsString());
                        member_tokens.Push(unhandled_tokens.Pop());
                        member_tokens.Push(unhandled_tokens.Pop());
                        if (unhandled_tokens.Count > 0)
                        {
                            member_tokens.Push(unhandled_tokens.Pop());
                        }
                        Assertion.Assert(unhandled_tokens.Count == 0, "Encountered unhandled tokens after reading member "
                            + member_tokens.Peek() + " while reading class " + cur_class.Name.AsString());

                        ReadMemberTokens(ref cur_member, ref member_tokens);
                        cur_member.Attributes = property_attributes;
                        property_attributes = new List<AttributeInstanceDescription>();
                        cur_members.Add(cur_member);
                        cur_member = new TypeInstanceDescription.MemberDescription();
                    }
                }
                else
                {
                    if (readState == ReadState.Attributes)
                    {
                        cur_attributes.Add(new AttributeInstanceDescription(new HashString(token)));
                    }
                    else
                    {
                        unhandled_tokens.Push(token);
                    }
                }
            }

            Assertion.Assert(unhandled_tokens.Count == 0, "Should have handled all tokens by now");

            return retVal;
        }
        private enum ReadState
        {
            Attributes,
            Class,
            Property, //member
            Value,
            Method,
            None //Count
        }
    }
}
