using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Exporters
{
    internal class CppExporter : IExporter
    {
        public abstract class IOutputSectionProperty
        {
            IOutputSection Parent;
            public IOutputSectionProperty(ref IOutputSection parent) {  Parent = parent; }
            public abstract string PropertyStringOutput();
        }
        public abstract class IOutputSection
        {
            protected OutputFile Parent;
            protected List<IOutputSectionProperty> Children;

            public IOutputSection(ref OutputFile parent)
            {
                Parent = parent;
                Children = null;
            }
            public IOutputSection(ref OutputFile parent, List<IOutputSectionProperty> childen) 
            {
                Parent = parent;
                Children = childen;
            }

            public abstract string SectionStringOutput();
        }
        public class ReferenceOutputSection : IOutputSection
        {
            private class ReferenceProperty : IOutputSectionProperty
            {
                public string RefPath { get; set; }
                public ReferenceProperty(ref IOutputSection parent, string ref_path) : base(ref parent)
                {
                    RefPath = ref_path;
                }

                public override string PropertyStringOutput()
                {
                    return "#include<" + RefPath + ".h>\n";
                }
            }
            private List<IOutputSectionProperty> CreateReferencePropertiesList(OutputFile parent)
            {
                List<IOutputSectionProperty> retval = new List<IOutputSectionProperty>();

                return retval;
            }
            public ReferenceOutputSection(ref OutputFile parent) : base(ref parent)
            {
                Children = CreateReferencePropertiesList(parent);
            }
            public override string SectionStringOutput()
            {
                string retval = "";
                foreach (IOutputSectionProperty prop in Children)
                {
                    retval += prop.PropertyStringOutput();
                }

                return retval;
            }
        }
        public class OutputFile
        {
            public OutputFile() { }
        }

        public void Export(in ExportSettings settings, in List<TypeInstanceDescription> type_desc_list)
        {
            Assertion.SoftAssert(false, "Cpp exporter does not implement type instance exporting!");
        }
        bool NeedsExport(TypeDefinition type_def, in DataMap dm)
        {
            return type_def.FindAttributeByName(new HashString("FilePath")) == null;
        }
        void Log(string message)
        {
            if (true)
            {
                Logging.Log(message);
            }
        }
        public void Export(in ExportSettings settings, in DataMap dm)
        {
            //no need to export attr defs for cpp
            //only type defs
            Uri input_path = new Uri(settings.InputDirectory);
            Uri output_path = new Uri(settings.OutputDirectory);

            Dictionary<HashString, List<TypeDefinition>> file_type_def_dictionary = new Dictionary<HashString, List<TypeDefinition>>();

            foreach(TypeDefinition type_def in dm.TypeDefinitions)
            {
                AttributeTag file_path_attr = type_def.FindAttributeByName(new HashString("FilePath"));
                if (file_path_attr != null)
                {
                    string file_path_val = file_path_attr.Values[0];
                    Uri file_path = new Uri(file_path_val);
                    string relative_file_path = file_path.MakeRelative(input_path);
                    string output_file_path = output_path.AbsolutePath + relative_file_path;
                    HashString output_file_path_hs = new HashString(output_file_path);

                    if (file_type_def_dictionary.ContainsKey(output_file_path_hs) == false)
                    {
                        file_type_def_dictionary[output_file_path_hs] = new List<TypeDefinition>();
                    }
                    file_type_def_dictionary[output_file_path_hs].Add(type_def);
                }
            }
        }
    }
}
