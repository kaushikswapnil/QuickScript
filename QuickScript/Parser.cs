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
            List<AttributeInstanceDescription> attributes = new List<AttributeInstanceDescription>();

            void ChangeReadState(ReadState newState)
            {
                prevState = readState;
                readState = newState;
            }

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
