#include "pch.h"
#include "QuickScript.h"
#include "QSParser.h"
#include "QSExporter.h"
#include <fstream>
#include <boost/archive/text_oarchive.hpp>
#include <boost/archive/text_iarchive.hpp>

QuickScript::QuickScript(const QuickScriptInitParams& params) : m_InitParams(params)
{
	InitializeTypeMap(params);

	std::vector<TypeInstanceDescription> extracted_types;
	for (const auto& entry : std::filesystem::directory_iterator(params.ReadDirectoryPath))
	{
		if (entry.path().extension() == ".qs")
		{
			QSParser::ParseFile(entry, extracted_types);
		}
	}

	for (const auto& entry : extracted_types)
	{
		if (!IsExistingTypeDefinition(entry))
		{
			TypeDefinitionHandleContainer members;
			TypeDefinitionMemberAttributesContainer member_attributes;
			TypeDefinitionMemberNamesContainer member_names;
			members.reserve(entry.m_Members.size());
			member_attributes.reserve(entry.m_Members.size());
			member_names.reserve(entry.m_Members.size());
			for (const auto& type_instance_member : entry.m_Members)
			{
				auto instance_member_handle = FindHandleFor(type_instance_member.m_TypeName);
				if (instance_member_handle != INVALID_TYPE_DEFINITION_HANDLE)
				{
					members.push_back(instance_member_handle);
					member_attributes.push_back(type_instance_member.m_Attributes);
					member_names.push_back(type_instance_member.m_Name);
				}
				else
				{
					HARDASSERT(false, "We should have an valid type def here!");
				}
			}
			InsertType(entry.m_Name, entry.m_Filename, entry.m_Attributes, members, member_names, member_attributes);
		}
	}

	OutputTypeMapAsCpp();
}

QuickScript::~QuickScript()
{
	WriteTypeMap();
}

void QuickScript::InitializeTypeMap(const QuickScriptInitParams& params)
{
	const std::filesystem::path type_map_file_path = GetTypeMapDefinitionsFilePath();
	std::ifstream input;
	input.open(type_map_file_path.generic_string().c_str(), std::ios::in);

	if (!input.is_open())
	{
		GenerateTypeMapFromScratch();
	}
	else
	{
		boost::archive::text_iarchive ia(input);
		ia >> m_TypeMap;
	}
}

void QuickScript::InsertType(const HashString& name, const std::string& qs_file_name, const AttributeContainer& attr_cont,
		const TypeDefinitionHandleContainer& members, const TypeDefinitionMemberNamesContainer& member_names, const TypeDefinitionMemberAttributesContainer& member_attr,
		TypeMap& out_to_type_map)
{
	out_to_type_map.m_Definitions.emplace_back(name, qs_file_name, attr_cont, members, member_names, member_attr);
	out_to_type_map.m_NameHashToHandleMap[name] = static_cast<TypeDefinitionHandle>(out_to_type_map.m_Definitions.size() - 1);
}

void QuickScript::GenerateTypeMapFromScratch()
{
	InsertType(HashString{ "char" }, HashString::InvalidHashString(), {}, {}, {}, {});
	InsertType(HashString{ "bool" }, HashString::InvalidHashString(), {}, {}, {}, {});
	InsertType(HashString{ "int" }, HashString::InvalidHashString(), {}, {}, {}, {});
	InsertType(HashString{ "float" }, HashString::InvalidHashString(), {}, {}, {}, {});
}

void QuickScript::WriteTypeMap()
{
	const std::filesystem::path type_map_file_path = GetTypeMapDefinitionsFilePath();
	std::ofstream output;
	output.open(type_map_file_path.generic_string().c_str(), std::ios::out);

	boost::archive::text_oarchive oa(output);
	oa << m_TypeMap;
}

void QuickScript::OutputTypeMapAsCpp() const
{
	CppQSExporter exporter{};
	exporter.ExportTypeMap(m_TypeMap, m_InitParams.ReadDirectoryPath, m_InitParams.OutputDirectory);
}

std::filesystem::path QuickScript::GetTypeMapDefinitionsFilePath() const
{
	std::filesystem::path type_map_file_path = m_InitParams.TypeMapDirectory;
	type_map_file_path.append("typemapdef.qstmdef");
	return type_map_file_path;
}

bool QuickScript::IsExistingTypeDefinition(const TypeInstanceDescription& instance_desc) const
{
	return IsExistingTypeDefinition(instance_desc.m_Name);
}

bool QuickScript::IsExistingTypeDefinition(const HashString& type_name) const
{
	return FindHandleFor(type_name) != INVALID_TYPE_DEFINITION_HANDLE;
}

TypeDefinitionHandle QuickScript::FindHandleFor(const TypeInstanceDescription& instance_desc) const
{
	return FindHandleFor(instance_desc.m_Name);
}

TypeDefinitionHandle QuickScript::FindHandleFor(const HashString& type_name) const
{
	auto handle_map_entry = m_TypeMap.m_NameHashToHandleMap.find(type_name);
	if (handle_map_entry != m_TypeMap.m_NameHashToHandleMap.end())
	{
		return handle_map_entry->second;
	}

	return INVALID_TYPE_DEFINITION_HANDLE;
}