using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript
{
    public class Parser
    {
        public static List<TypeInstanceDescription> Parse(in string descLines)
        {
            List<string> tokens = new List<string>();
            string curWord = "";

            foreach (char c in descLines) 
            {
                switch (c)
                {
                    case ' ':
                    case '\n':
                    case '\t':
                        if (curWord.Length > 0)
                        {
                            tokens.Add(curWord);
                            curWord = "";
                        }
                        break;
                    case ';':
                    case '{':
                    case '}':
                    case '[':
                    case ']':
                        if (curWord.Length > 0)
                        {
                            tokens.Add(curWord);
                            curWord = "";
                        }
                        tokens.Add(Char.ToString(c));
                        break;
                    case '=':
                        break; //we ignore the equal symbol
                    default:
                        curWord += c;
                        break;
                }

                if (curWord.Length > 0)
                {
                    tokens.Add(curWord);
                }
            }

            return ExtractTypes(tokens);
        }

        static public List<TypeInstanceDescription> ExtractTypes(in List<string> tokens)
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
            List<TypeInstanceDescription> cur_members = new List<TypeInstanceDescription>();
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
                    Assertion.Assert(unhandled_tokens.Count == 0, "Should already have handled all tokens!");                    
                    
                    if (cur_members.Count > 0)
                    {
                        cur_class.Members = cur_members;
                        cur_members = new List<TypeInstanceDescription>();
                    }
                    
                    retVal.Add(cur_class);
                    cur_class = new TypeInstanceDescription();
                }
                else if (token == ";")
                {
                    Assertion.Assert(readState == ReadState.Member, "Should only encounter ; when reading members");
                    Assertion.Assert(unhandled_tokens.Count > 1, "Should have atleast the member type and name here");
                    if (unhandled_tokens.Count > 2)
                    {
                        //type, name, val
                        cur_member.Value = unhandled_tokens.Pop(); 
                    }
                    //type, name
                    cur_member.TypeDescription.Name.Reset(unhandled_tokens.Pop());
                    cur_member.Name.Reset(unhandled_tokens.Pop());
                }
                else 
                {
                    if (readState == ReadState.Attributes)
                    {
                        cur_attributes.Add(new AttributeInstanceDescription(token));
                    }
                    else
                    {
                        unhandled_tokens.Push(token);
                    }
                }
            }

            return new List<TypeInstanceDescription>();
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
