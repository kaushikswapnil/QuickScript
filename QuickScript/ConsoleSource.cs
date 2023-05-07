using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using QuickScript.Utils;
using QuickScript.Exporters;
using QuickScript.Testing;

namespace QuickScript
{
    internal class ConsoleSource
    {
        public class Employee
        {
            public HashString Name { get; set; } = new HashString("Whatever");
            public Employee? Manager { get; set; }
            public List<Employee>? DirectReports { get; set; }
        }
        static public void Main(string[] args)
        {
            Testing.TestsSuite test = new TestsSuite();
        }
    }
}
