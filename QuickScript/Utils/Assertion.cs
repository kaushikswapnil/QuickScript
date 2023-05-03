using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Utils
{
    public class Assertion
    {
        static public void Warn(string message,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Logging.Log("[Warning]" + filePath + ":" + lineNumber + "]" + message, "", 0);
        }

        static public void Assert<T>(bool condition, string message = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0) where T : Exception
        {
            if (condition == false)
            {
                Logging.Log("[ERROR][" + filePath + ":" + lineNumber + "]" + message, "", 0);
                throw Activator.CreateInstance(typeof(T), message) as T;
            }
        }

        static public void Assert(bool condition, string message = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Assert<Exception>(condition, message, filePath, lineNumber);
        }

        static public void SoftAssert(bool condition, string message = "",
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            try
            {
                Assert(condition, message, filePath, lineNumber);
            }
            catch (Exception e) 
            { 
                //do nothing
            }
        }
    }
}
