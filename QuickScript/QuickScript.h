#pragma once
#include <filesystem>
#include "Type.h"

struct QuickScriptInitParams
{
	std::filesystem::path ReadDirectoryPath;
};


class QuickScript
{
public:
	QuickScript(const QuickScriptInitParams& params);
	~QuickScript() = default;

private:
	void InitializeTypeMap();
	void InsertType(const HashString& name,
		const HashString& qs_file_name,
		const AttributeContainer& attr_cont,
		const Value& def_value,
		const TypeDefinitionHandleContainer& members,
		const std::vector<AttributeContainer>& member_attr);

	TypeMap m_TypeMap;
};

