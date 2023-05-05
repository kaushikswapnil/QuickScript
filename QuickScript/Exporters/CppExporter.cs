using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Exporters
{
    internal class CppExporter : IExporter
    {
        public void Export(in ExportSettings settings, in List<TypeInstanceDescription> type_desc_list)
        {
            Assertion.SoftAssert(false, "Cpp exporter does not implement type instance exporting!");
        }
        public void Export(in ExportSettings settings, in DataMap dm)
        {

        }
    }
}
