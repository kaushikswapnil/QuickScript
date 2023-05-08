using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Exporters
{
    internal class CppExporter : IExporter
    {
        private static string GetOutputUnitPath(string absolute_input_file_path, in ExportSettings settings)
        {
            Uri input_path = new Uri(settings.InputDirectory);
            Uri output_path = new Uri(settings.OutputDirectory);
            Uri file_path = new Uri(absolute_input_file_path);
            string relative_file_path = file_path.MakeRelative(input_path);
            string output_file_path = output_path.AbsolutePath + relative_file_path;
            return output_file_path;
        }
        //returns null if this class does not need an output 
        private static string? GetTypeDefinitionOutputUnitPath(TypeDefinition type, in ExportSettings settings)
        {
            AttributeTag file_path_attr = type.FindAttributeByName(new HashString("FilePath"));
            if (file_path_attr != null)
            {
                Assertion.Assert(file_path_attr != null, "Method called with " + type.GetName() + " which does not have file path attribute! Should only be called with methods that have that attribute");
                string file_path_val = file_path_attr.Values[0];
                return GetOutputUnitPath(file_path_val, settings);
            }

            return null;
        }
        public abstract class IOutputSection
        {
            protected IOutputFile Parent;

            public IOutputSection(ref IOutputFile parent)
            {
                Parent = parent;
            }

            public abstract string SectionStringOutput();
        }
        public abstract class IOutputFile
        {
            protected OutputUnit Parent;
            protected List<IOutputSection> Children;
            public IOutputFile(OutputUnit parent) 
            { 
                Parent = parent;
                Children = new List<IOutputSection>();
            }
            public virtual string FileOutput() 
            {
                string retval = "";
                foreach (IOutputSection sec in Children)
                {
                    retval += sec.SectionStringOutput();
                }
                return retval;
            }
            public abstract void WriteFile();
        }

        public class OutputHeaderFile : IOutputFile
        {
            public OutputHeaderFile(OutputUnit parent, List<TypeDefinition> defs_in_file) : base(parent) 
            {
            }
            public override void WriteFile()
            {
                throw new NotImplementedException();
            }
        }

        public class OutputCppFile : IOutputFile
        {
            public OutputCppFile(OutputUnit parent, List<TypeDefinition> defs_in_file) : base(parent) { }
            public override void WriteFile()
            {
                throw new NotImplementedException();
            }
        }

        public class OutputUnit
        {
            public List<TypeDefinition> TypeDefinitionsInFile { get; set; }
            public OutputHeaderFile HeaderFile;
            public OutputCppFile CppFile;
            public HashString FullName; //Is with full output path
            public OutputUnit(HashString full_name, List<TypeDefinition> type_def_list) 
            {
                FullName = full_name;
                TypeDefinitionsInFile = type_def_list;
                HeaderFile = new OutputHeaderFile(this, type_def_list);
                CppFile = new OutputCppFile(this, type_def_list);
            }
        }

        public override void Export(in ExportSettings settings, in List<TypeInstanceDescription> type_desc_list)
        {
            Assertion.SoftAssert(false, "Cpp exporter does not implement type instance exporting!");
        }
        void Log(string message)
        {
            if (true)
            {
                Logging.Log(message);
            }
        }
        public override void Export(in ExportSettings settings, in DataMap dm)
        {
            //no need to export attr defs for cpp
            //only type defs
            Dictionary<HashString, List<TypeDefinition>> file_type_def_dictionary = new Dictionary<HashString, List<TypeDefinition>>();

            DebugLog(settings, "Collecting all type defs to export");
            foreach(TypeDefinition type_def in dm.TypeDefinitions)
            {
                string? potential_output_path = GetTypeDefinitionOutputUnitPath(type_def, settings);
                if (potential_output_path != null)
                {
                    HashString output_hs = new HashString(potential_output_path);

                    if (file_type_def_dictionary.ContainsKey(output_hs) == false)
                    {
                        file_type_def_dictionary.Add(output_hs, new List<TypeDefinition>());
                    }

                    file_type_def_dictionary[output_hs].Add(type_def);
                }
            }

            DebugLog(settings, "Collected file path - type defs dictionary " + file_type_def_dictionary);

            foreach (KeyValuePair<HashString, List<TypeDefinition>> file_type_def_pair in file_type_def_dictionary)
            {

            }
        }
    }
}
