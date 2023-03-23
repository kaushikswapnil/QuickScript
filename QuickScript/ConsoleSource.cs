using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript
{
    internal class ConsoleSource
    {
        static public void Main(string[] args)
        {
            Parser.Parse("[whatever, hello]\nclass NewPuru\n{\n[kya]\nint new_mem = 10;\n}\n");
        }
    }
}
