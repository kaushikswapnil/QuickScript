using QuickScript.Typing;
using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript
{
    using ValueType = QuickScript.Typing.ValueType;
    public class Parser
    {
        public static List<TypeInstanceDescription> ParseDirectory(in string directory_name, in bool parse_sub_directories = true)
        {
            Assertion.Assert(Directory.Exists(directory_name), "Directory to parse does not exist!");

            List<TypeInstanceDescription> retval = new List<TypeInstanceDescription>();

            var file_names = Directory.EnumerateFiles(directory_name,"*.qs",
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
                        if (!reading_comment)
                        {
                            TryAddProperWordToTokens();
                            tokens.Add(Char.ToString(c));
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
            List<TypeInstanceDescription.MemberDescription> cur_members = new List<TypeInstanceDescription.MemberDescription>();
            TypeInstanceDescription.MemberDescription cur_member = new TypeInstanceDescription.MemberDescription();

            foreach (string token in tokens)
            {
                if (token == "[")
                {
                    Assertion.Assert(readState == ReadState.Class || readState == ReadState.Member, "Invalid flow. Read attributes before class or member");
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
                            Assertion.Assert(prevState == ReadState.Member, "Should have been reading a member");
                            cur_member.Attributes = cur_attributes;
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
                    ChangeReadState(ReadState.Member);
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
                }
                else if (token == ";")
                {
                    Assertion.Assert(readState == ReadState.Member, "Should only encounter ; when reading members");
                    Assertion.Assert(unhandled_tokens.Count > 1, "Should have atleast the member type and name here");
                    if (unhandled_tokens.Count > 2)
                    {
                        //type, name, val
                        cur_member.Value = new ValueType(unhandled_tokens.Pop()); 
                    }
                    //type, name
                    cur_member.Name.Reset(unhandled_tokens.Pop());
                    cur_member.TypeName.Reset(unhandled_tokens.Pop());

                    cur_members.Add(cur_member);
                    cur_member = new TypeInstanceDescription.MemberDescription();
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
            Member,
            Value,
            None //Count
        }
    }
}
