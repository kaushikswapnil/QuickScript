#include "pch.h"
#include "QuickScript.h"
#include "QSParser.h"
#include <fstream>
#include <boost/archive/text_oarchive.hpp>
#include <boost/archive/text_iarchive.hpp>

QuickScript::QuickScript(const QuickScriptInitParams& params) : m_InitParams(params)
{
	InitializeTypeMap(params);

	std::vector<TypeInstanceDescription> extracted_types;
	for (const auto& entry : std::filesystem::directory_iterator(params.ReadDirectoryPath))
	{
		QSParser::ParseFile(entry, extracted_types);
	}

	for (const auto& entry : extracted_types)
	{
		if (!IsValidTypeInstanceDefinition(entry))
		{
			InsertType(entry.m_Name, entry.m_Filename, entry.m_Attributes, {}, {}, {});
		}
	}
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

void QuickScript::InsertType(const HashString& name, const HashString& qs_file_name, const AttributeContainer& attr_cont, const Value& def_value,
		const TypeDefinitionHandleContainer& members, const std::vector<AttributeContainer>& member_attr,
		TypeMap& out_to_type_map)
{
	out_to_type_map.m_Definitions.emplace_back(name, qs_file_name, attr_cont, def_value, members, member_attr);
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

std::filesystem::path QuickScript::GetTypeMapDefinitionsFilePath() const
{
	std::filesystem::path type_map_file_path = m_InitParams.TypeMapDirectory;
	type_map_file_path.append("typemapdef.qstmdef");
	return type_map_file_path;
}

bool QuickScript::IsValidTypeInstanceDefinition(const TypeInstanceDescription& instance_desc) const
{
	return IsValidTypeInstanceDefinition(instance_desc.m_Name);
}

bool QuickScript::IsValidTypeInstanceDefinition(const HashString& type_name) const
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