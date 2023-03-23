using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Utils
{
    public class Logging
    {
        static public void LogDomain(string domain, string message,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
        {
            string final_log = "";
            final_log +=  domain != "" ? "[" + domain  + "]": "";
            final_log += filePath != "" ? "[" + filePath + ":" + lineNumber.ToString() + "]" : "";
            final_log += message;
            Console.WriteLine(final_log);
        }

        static public void Log(string message,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            LogDomain("", message, filePath, lineNumber);
        }
    }
}
