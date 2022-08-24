#include "pch.h"
#include "QuickScript.h"
#include "QSParser.h"

QuickScript::QuickScript(const QuickScriptInitParams& params)
{


	for (const auto& entry : std::filesystem::directory_iterator(params.ReadDirectoryPath))
	{
		QSParser::ParseFile(entry);
	}
}

void QuickScript::InitializeTypeMap()
{
	InsertType(HashString{"char"}, HashString::InvalidHashString(), {}, {}, {}, {});
	InsertType(HashString{ "bool" }, HashString::InvalidHashString(), {}, {}, {}, {});
	InsertType(HashString{ "int" }, HashString::InvalidHashString(), {}, {}, {}, {});
	InsertType(HashString{ "float" }, HashString::InvalidHashString(), {}, {}, {}, {});
}

void QuickScript::InsertType(const HashString& name, const HashString& qs_file_name, const AttributeContainer& attr_cont, const Value& def_value, const TypeDefinitionHandleContainer& members, const std::vector<AttributeContainer>& member_attr)
{
	m_TypeMap.m_Definitions.emplace_back(name, qs_file_name, attr_cont, def_value, members, member_attr);
}

