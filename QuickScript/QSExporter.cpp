#include "QSExporter.h"
#include <fstream>

typedef std::string FileSection;

//qs generated 
struct QSGeneratedFileSectionTagParams
{
	bool m_IsUserContentArea{false};
	bool m_IsFileHeaderSection{false};
};

static const std::string QS_GEN_AREA_BEGIN{ "QS_GENERATED_AREA\n" };
static const std::string QS_USER_AREA_BEGIN{ "QS_USER_AREA\n" };
static const std::string QS_GEN_AREA_END{ "QS_GENERATED_AREA\n" };
static const std::string QS_USER_AREA_END{ "QS_USER_AREA\n" };
void InsertQSFileSectionHeader(FileSection& section, const QSGeneratedFileSectionTagParams& params)
{
	if (params.m_IsUserContentArea == false)
	{
		section += QS_GEN_AREA_BEGIN;
	}
	else
	{
		section += QS_USER_AREA_BEGIN;
	}
}

void InsertQSFileSectionFooter(FileSection& section, const QSGeneratedFileSectionTagParams& params)
{
	if (params.m_IsUserContentArea == false)
	{
		section += QS_GEN_AREA_END;
	}
	else
	{
		section += QS_USER_AREA_END;
	}
}

void QSExporter::ExportTypeMap(const TypeMap& type_map, const std::filesystem::path& read_directory, const std::filesystem::path& output_directory)
{
	for (const auto& entry : type_map.m_Definitions)
	{
		if (!entry.IsPrimitive())
		{
			std::filesystem::path rel_path = std::filesystem::relative(std::filesystem::path(entry.m_QSFileName), read_directory);
			std::filesystem::path cpp_file_path = output_directory;
			cpp_file_path /= rel_path.parent_path();

			//currently it points to the parent folder
			if (!std::filesystem::exists(cpp_file_path))
			{
				std::filesystem::create_directory(cpp_file_path);
			}
			cpp_file_path /= entry.m_Name.AsString();
			cpp_file_path.replace_extension(".h");

			std::ofstream cpp_file{ cpp_file_path, std::ofstream::out | std::ofstream::binary };

			std::vector<FileSection> file_sections{};

			//top section
			//should have pragmas
			//code gen macro defs
			//include files in headers/cpp
			{
				FileSection section{""};
				QSGeneratedFileSectionTagParams params{};
				InsertQSFileSectionHeader(section, params);

				section += "#pragma once\n";

				InsertQSFileSectionFooter(section, params);
				file_sections.push_back(section);
			}

			//class name section
			//should be struct if struct
			//have inheritance as needed
			{
				FileSection section{ "" };
				QSGeneratedFileSectionTagParams params{};
				InsertQSFileSectionHeader(section, params);

				section += "class " + entry.m_Name.AsString() + "\n{\n";

				InsertQSFileSectionFooter(section, params);
				file_sections.push_back(section);
			}

			//public section. should have getters and setters for all member variables
			{
				FileSection section{ "" };
				QSGeneratedFileSectionTagParams params{};
				InsertQSFileSectionHeader(section, params);

				section += "public:\n";
				for (auto member_iter = 0; member_iter < entry.m_Members.size(); ++member_iter)
				{
					const auto member_handle = entry.m_Members[member_iter];
					const auto member_type_name = type_map.m_Definitions[member_handle].m_Name.AsString();
					const auto member_name = entry.m_MemberNames[member_iter].AsString();

					//const getter
					section += "\tinline const " + member_type_name + "& Get" + member_name + "() const { return " + member_name + "; }\n";
					//getter
					section += "\tinline " + member_type_name + "& Get" + member_name + "() { return " + member_name + "; }\n";
					//setter
					section += "\tinline Set" + member_name + "(const " + member_type_name + "& val) { " + member_name + " = val; }\n";
				}

				InsertQSFileSectionFooter(section, params);
				file_sections.push_back(section);
			}

			//private section
			//all variables
			{
				FileSection section{ "" };
				QSGeneratedFileSectionTagParams params{};
				InsertQSFileSectionHeader(section, params);

				section += "private:\n";
				for (auto member_iter = 0; member_iter < entry.m_Members.size(); ++member_iter)
				{
					const auto member_handle = entry.m_Members[member_iter];
					const auto member_type_name = type_map.m_Definitions[member_handle].m_Name.AsString();
					const auto member_name = entry.m_MemberNames[member_iter].AsString();

					section += "\t " + member_type_name + " " + member_name + ";\n";
				}

				InsertQSFileSectionFooter(section, params);
				file_sections.push_back(section);
			}

			//user def section
			{
				FileSection section{ "" };
				QSGeneratedFileSectionTagParams params{};
				params.m_IsUserContentArea = true;
				InsertQSFileSectionHeader(section, params);
				InsertQSFileSectionFooter(section, params);
				file_sections.push_back(section);
			}

			//closing section
			{
				FileSection section{ "" };
				QSGeneratedFileSectionTagParams params{};
				InsertQSFileSectionHeader(section, params);
				section += "};";
				InsertQSFileSectionFooter(section, params);
				file_sections.push_back(section);
			}
			
			for (const auto& section : file_sections)
			{
				cpp_file << section << '\n';
			}

			cpp_file.close();
		}
	}
}