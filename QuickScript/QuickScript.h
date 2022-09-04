#pragma once
#include <filesystem>
#include "Type.h"

struct QuickScriptInitParams
{
	std::filesystem::path ReadDirectoryPath;
	std::filesystem::path TypeMapDirectory;
	std::filesystem::path AttributeDirectory;
};


class QuickScript
{
public:
	QuickScript(const QuickScriptInitParams& params);
	~QuickScript();

private:
	void InitializeTypeMap(const QuickScriptInitParams& params);
	void GenerateTypeMapFromScratch();
	void WriteTypeMap();

	std::filesystem::path GetTypeMapDefinitionsFilePath() const;

	void InsertType(const HashString& name,
		const HashString& qs_file_name,
		const AttributeContainer& attr_cont,
		const Value& def_value,
		const TypeDefinitionHandleContainer& members,
		const std::vector<AttributeContainer>& member_attr);

	TypeMap m_TypeMap;
	QuickScriptInitParams m_InitParams;
};

