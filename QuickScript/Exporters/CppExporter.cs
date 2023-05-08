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
        private static readonly string GENERATED_SECTION_HEADER = "QS_GS_BEGIN\n";
        private static readonly string GENERATED_SECTION_FOOTER = "QS_GS_END\n";
        private static readonly string USER_SECTION_HEADER = "QS_US_BEGIN\n";
        private static readonly string USER_SECTION_FOOTER = "QS_US_END\n";

        private static string GetOutputUnitPath(string absolute_input_file_path, in ExportSettings settings)
        {
            Uri input_path = new Uri(settings.InputDirectory);
            Uri output_path = new Uri(settings.OutputDirectory);
            Uri file_path = new Uri(absolute_input_file_path);
            string relative_file_path = input_path.MakeRelative(file_path);
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
        private static string ReplaceQsFileExtensionWith(string file_path, string extension)
        {
            return file_path.Replace(".qs", extension);
        }
        private static string ConvertOutputUnitPathToHeaderFilePath(string path)
        {
            return ReplaceQsFileExtensionWith(path, ".h");
        }
        private static string ConvertOutputUnitPathToCppFilePath(string path)
        {
            return ReplaceQsFileExtensionWith(path, ".cpp");
        }
        public abstract class IOutputSection
        {
            protected OutputFile Parent;
            public string UserSectionData = "";
            public IOutputSection(ref OutputFile parent)
            {
                Parent = parent;
            }

            public abstract string SectionStringOutput();
        }
        public class IncludeGuardsSection : IOutputSection
        {
            public IncludeGuardsSection(ref OutputFile parent) : base(ref parent)
            {
            }
            public override string SectionStringOutput()
            {
                string retval = GENERATED_SECTION_HEADER;
                retval += "#pragma once\n";
                retval += GENERATED_SECTION_FOOTER;
                return retval;
            }
        }
        public class ReferenceSection : IOutputSection
        {
            List<string> References;
            public ReferenceSection(ref OutputFile parent, List<string> references) : base(ref parent)
            {
                References = references;
            }
            public override string SectionStringOutput()
            {
                string retval = GENERATED_SECTION_HEADER;
                foreach (string reference_path in References)
                {
                    retval += "#include<" + reference_path + ">\n";
                }
                retval += GENERATED_SECTION_FOOTER;
                return retval;
            }
        }
        public class ClassDeclarationSection : IOutputSection
        {
            public TypeDefinition FromType;
            public ClassDeclarationSection(ref OutputFile parent, TypeDefinition from_def) : base(ref parent)
            {
                FromType = from_def;
            }
            public override string SectionStringOutput()
            {
                string retval = GENERATED_SECTION_HEADER;
                retval += "class " + FromType.GetName() + "\n{\n";
                string indent = new string(' ', (int)(4));

                if (FromType.HasMembers())
                {
                    string protected_member_decls = "protected:\n";
                    foreach (TypeDefinition.MemberDefinition memberDefinition in FromType.Members)
                    {
                        protected_member_decls += indent + memberDefinition.TypeName.AsString() + " " + memberDefinition.Name.AsString();
                        if (memberDefinition.HasValue())
                        {
                            protected_member_decls += " = " + memberDefinition.Value.AsString();
                        }
                        protected_member_decls += ";\n";
                    }
                    retval += protected_member_decls;
                }                

                retval += GENERATED_SECTION_FOOTER;
                //user section
                {
                    retval += USER_SECTION_HEADER;
                    retval += USER_SECTION_FOOTER;
                }
                retval += GENERATED_SECTION_HEADER;
                retval += "};\n";
                retval += GENERATED_SECTION_FOOTER;
                return retval;
            }
        }
        public class OutputFile
        {
            protected OutputUnit Parent;
            public List<IOutputSection> Sections;
            public string FilePath;
            public OutputFile(OutputUnit parent, string filepath) 
            { 
                Parent = parent;
                Sections = new List<IOutputSection>();
                FilePath = filepath;
            }
            public string FileOutput() 
            {
                string retval = "";
                foreach (IOutputSection sec in Sections)
                {
                    retval += sec.SectionStringOutput();
                }
                return retval;
            }
            public void WriteFile()
            {
                FileInfo fileInfo = new FileInfo(FilePath);
                if (!fileInfo.Directory.Exists)
                    fileInfo.Directory.Create();
                File.WriteAllText(FilePath, FileOutput());
            }
        }
        public class OutputUnit
        {
            public List<TypeDefinition> TypeDefinitionsInFile { get; set; }
            public OutputFile HeaderFile;
            public OutputFile CppFile;
            public HashString FullName; //Is with full output path
            public OutputUnit(HashString full_name, List<TypeDefinition> type_def_list, DataMap dm, in ExportSettings settings) 
            {
                FullName = full_name;
                TypeDefinitionsInFile = type_def_list;
                string header_file_path = ConvertOutputUnitPathToHeaderFilePath(full_name.AsString());
                HeaderFile = new OutputFile(this, header_file_path);
                string cpp_file_path = ConvertOutputUnitPathToCppFilePath(full_name.AsString());
                CppFile = new OutputFile(this, cpp_file_path);

                List<string> references_in_header_file = new List<string>();
                List<ClassDeclarationSection> classes_in_header = new List<ClassDeclarationSection>();
                
                foreach (TypeDefinition type_def in type_def_list)
                {
                    classes_in_header.Add(new ClassDeclarationSection(ref HeaderFile, type_def));

                    if (type_def.HasMembers())
                    {
                        foreach (TypeDefinition.MemberDefinition memberDefinition in type_def.Members)
                        {
                            TypeDefinition? member_type_def = dm.GetTypeDefinitionByName(memberDefinition.TypeName);
                            Assertion.Assert(member_type_def != null, "Should always have member here!");
                            string? member_type_def_file_path = GetTypeDefinitionOutputUnitPath(member_type_def, settings);
                            if (member_type_def_file_path != null)
                            {
                                //this member has a type def and we should reference it
                                references_in_header_file.Add(ConvertOutputUnitPathToHeaderFilePath(member_type_def_file_path));
                            }
                        }
                    }
                }
                //construct header file
                {
                    HeaderFile.Sections.Add(new IncludeGuardsSection(ref HeaderFile));
                    HeaderFile.Sections.Add(new ReferenceSection(ref HeaderFile, references_in_header_file));
                    foreach (ClassDeclarationSection sec in classes_in_header)
                    {
                        HeaderFile.Sections.Add(sec);
                    }
                }
                //construct cpp file
                {
                    CppFile.Sections.Add(new ReferenceSection(ref CppFile, new List<string> { HeaderFile.FilePath }));
                }
            }
            public void WriteUnit(in ExportSettings settings)
            {
                IExporter.DebugLog(settings, "Writing header file path " + HeaderFile.FilePath);
                HeaderFile.WriteFile();
                IExporter.DebugLog(settings, "Writing cpp file path " + CppFile.FilePath);
                CppFile.WriteFile();
            }
        }

        public override void Export(in ExportSettings settings, in List<TypeInstanceDescription> type_desc_list)
        {
            Assertion.SoftAssert(false, "Cpp exporter does not implement type instance exporting!");
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
                DebugLog(settings, "Creating and writing output unit with name " + file_type_def_pair.Key);
                OutputUnit output_unit = new OutputUnit(file_type_def_pair.Key, file_type_def_pair.Value, dm, settings);
                output_unit.WriteUnit(settings);
            }
        }
    }
}
