#include "pch.h"
#include "QuickScript.h"
#include "QSParser.h"
#include <fstream>
#include <boost/archive/text_oarchive.hpp>
#include <boost/archive/text_iarchive.hpp>

QuickScript::QuickScript(const QuickScriptInitParams& params)
{
	InitializeTypeMap(params);

	for (const auto& entry : std::filesystem::directory_iterator(params.ReadDirectoryPath))
	{
		QSParser::ParseFile(entry);
	}
}

void QuickScript::InitializeTypeMap(const QuickScriptInitParams& params)
{
	std::filesystem::path type_map_file_path = params.TypeMapDirectory;
	type_map_file_path.append("typemapdef.qstmdef");
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

void QuickScript::InsertType(const HashString& name, const HashString& qs_file_name, const AttributeContainer& attr_cont, const Value& def_value, const TypeDefinitionHandleContainer& members, const std::vector<AttributeContainer>& member_attr)
{
	m_TypeMap.m_Definitions.emplace_back(name, qs_file_name, attr_cont, def_value, members, member_attr);
}

void QuickScript::GenerateTypeMapFromScratch()
{
	InsertType(HashString{ "char" }, HashString::InvalidHashString(), {}, {}, {}, {});
	InsertType(HashString{ "bool" }, HashString::InvalidHashString(), {}, {}, {}, {});
	InsertType(HashString{ "int" }, HashString::InvalidHashString(), {}, {}, {}, {});
	InsertType(HashString{ "float" }, HashString::InvalidHashString(), {}, {}, {}, {});
}