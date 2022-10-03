#include "QSExporter.h"
#include <fstream>

typedef std::string FileSection;

enum class FileSectionTypes
{
	Header,
	Prequisites, //includes and forward decls
	ClassNameDeclaration, //name, inheritance methods
	LifetimeMethods, //constructor, destructor, copy, move, so on
	PublicMethods,
	PublicMembers,
	PrivateMethods,
	PrivateMembers,
	ProtectedMethods,
	ProtectedMembers,
	Footer,
	Count
};

static const std::string QS_GEN_AREA_BEGIN{ "QS_GENERATED_AREA_BEGIN\n" };
static const std::string QS_USER_AREA_BEGIN{ "QS_USER_AREA_BEGIN\n" };
static const std::string QS_GEN_AREA_END{ "QS_GENERATED_AREA_END\n" };
static const std::string QS_USER_AREA_END{ "QS_USER_AREA_END\n" };

struct FileSectionGenerationParams
{
	FileSectionGenerationParams(const TypeDefinition& assc_type, const TypeMap& type_map) : m_AssociatedType(assc_type), m_TypeMap(type_map) {}

	const TypeDefinition& m_AssociatedType;
	const TypeMap& m_TypeMap;
	FileSectionTypes m_FileSectionType;
};

void OpenQSSection(FileSection& section, const bool is_user_sec = false)
{
	if (!is_user_sec)
	{
		section += QS_GEN_AREA_BEGIN;
	}
	else
	{
		section += QS_USER_AREA_BEGIN;
	}
}

void CloseQSSection(FileSection& section, const bool is_user_sec = false)
{
	if (!is_user_sec)
	{
		section += QS_GEN_AREA_END;
	}
	else
	{
		section += QS_USER_AREA_END;
	}
}

void FillSection(FileSection& section, const FileSectionGenerationParams& gen_params)
{
	OpenQSSection(section);
	switch (gen_params.m_FileSectionType)
	{
		case FileSectionTypes::Header:
		{
			section += "#pragma once\n";
		}
		break;
		case FileSectionTypes::Prequisites:
		break;
		case FileSectionTypes::ClassNameDeclaration: 
		section += "class " + gen_params.m_AssociatedType.m_Name.AsString() + "\n{\n";

		break;
		case FileSectionTypes::LifetimeMethods: 
		break;
		case FileSectionTypes::PublicMethods: 
		{
			section += "public:\n";
			for (auto member_iter = 0; member_iter < gen_params.m_AssociatedType.m_Members.size(); ++member_iter)
			{
				const auto member_handle = gen_params.m_AssociatedType.m_Members[member_iter];
				const auto member_type_name = gen_params.m_TypeMap.m_Definitions[member_handle].m_Name.AsString();
				const auto member_name = gen_params.m_AssociatedType.m_MemberNames[member_iter].AsString();

				//const getter
				section += "\tinline const " + member_type_name + "& Get" + member_name + "() const { return " + member_name + "; }\n";
				//getter
				section += "\tinline " + member_type_name + "& Get" + member_name + "() { return " + member_name + "; }\n";
				//setter
				section += "\tinline Set" + member_name + "(const " + member_type_name + "& val) { " + member_name + " = val; }\n";
			}
		}
		break;
		case FileSectionTypes::PublicMembers:
		break;
		case FileSectionTypes::PrivateMethods:
		break;
		case FileSectionTypes::PrivateMembers:
		break;
		case FileSectionTypes::ProtectedMethods:
		break;
		case FileSectionTypes::ProtectedMembers:
		{
			section += "protected:\n";
			for (auto member_iter = 0; member_iter < gen_params.m_AssociatedType.m_Members.size(); ++member_iter)
			{
				const auto member_handle = gen_params.m_AssociatedType.m_Members[member_iter];
				const auto member_type_name = gen_params.m_TypeMap.m_Definitions[member_handle].m_Name.AsString();
				const auto member_name = gen_params.m_AssociatedType.m_MemberNames[member_iter].AsString();

				section += "\t " + member_type_name + " " + member_name + ";\n";
			}
		}
		break;
		case FileSectionTypes::Footer:
		section += "};\n";
		break;
		default:
		//HARDASSERT(false, "this should not have happened!");
		break;
	}
	CloseQSSection(section);
}

std::filesystem::path GetExportedFilePathForType(const TypeDefinition& type, const std::filesystem::path& read_directory, const std::filesystem::path& output_directory)
{
	std::filesystem::path rel_path = std::filesystem::relative(std::filesystem::path(type.m_QSFileName), read_directory);
	std::filesystem::path cpp_file_path = output_directory;
	cpp_file_path /= rel_path.parent_path();
	cpp_file_path /= type.m_Name.AsString();
	cpp_file_path.replace_extension(".h");
	return cpp_file_path;
}

void QSExporter::ExportTypeMap(const TypeMap& type_map, const std::filesystem::path& read_directory, const std::filesystem::path& output_directory)
{
	for (const auto& entry : type_map.m_Definitions)
	{
		if (!entry.IsPrimitive())
		{
			const std::filesystem::path cpp_file_path = GetExportedFilePathForType(entry, read_directory, output_directory);

			//currently it points to the parent folder
			if (!std::filesystem::exists(cpp_file_path.parent_path()))
			{
				std::filesystem::create_directory(cpp_file_path.parent_path());
			}

			std::ofstream cpp_file{ cpp_file_path, std::ofstream::out | std::ofstream::binary };

			std::vector<FileSection> file_sections{};

			FileSectionGenerationParams gen_params{entry, type_map};

			auto fill_section_wrapper = [&gen_params, &file_sections](const FileSectionTypes new_section)
			{
				FileSection section{""};
				gen_params.m_FileSectionType = new_section;
				FillSection(section, gen_params);
				file_sections.push_back(section);
			};

			for (auto iter = 0; iter < static_cast<uint32_t>(FileSectionTypes::Count); ++iter)
			{
				fill_section_wrapper(static_cast<FileSectionTypes>(iter));
			}
			
			for (const auto& section : file_sections)
			{
				cpp_file << section << '\n';
			}

			cpp_file.close();
		}
	}
}