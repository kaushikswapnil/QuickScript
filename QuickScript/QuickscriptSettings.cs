using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript
{
    public class QuickscriptSettings
    {
        static readonly public string DEFAULT_INPUT_TEST_DIRECTORY = "D:\\PersonalProjects\\QuickScript\\Tests\\Input";
        static readonly public string DEFAULT_OUTPUT_TEST_DIRECTORY = "D:\\PersonalProjects\\QuickScript\\Tests\\Output";
        static readonly public string DEFAULT_DATA_MAP_PATH = "D:\\PersonalProjects\\QuickScript\\Tests\\QSDataMap.qsdm";
        public string QuickScriptsDirectory = DEFAULT_INPUT_TEST_DIRECTORY;
        public string OutputDirectory = DEFAULT_OUTPUT_TEST_DIRECTORY;
        public string DataMapPath = DEFAULT_DATA_MAP_PATH;
    }
}
