#pragma once
#include <filesystem>
#include "Type.h"

struct QuickScriptInitParams
{
	std::filesystem::path ReadDirectoryPath;
	std::filesystem::path TypeMapDirectory;
	std::filesystem::path AttributeDirectory;
	std::filesystem::path OutputDirectory;
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

	void OutputTypeMapAsCpp() const;

	std::filesystem::path GetTypeMapDefinitionsFilePath() const;

	void InsertType(const HashString& name,
		const std::string& qs_file_name,
		const AttributeContainer& attr_cont,
		const Value& def_value,
		const TypeDefinitionHandleContainer& members,
		const std::vector<std::string>& member_names,
		const std::vector<AttributeContainer>& member_attr,
		TypeMap& out_to_type_map);
	void InsertType(const HashString& name,
		const std::string& qs_file_name,
		const AttributeContainer& attr_cont,
		const Value& def_value,
		const TypeDefinitionHandleContainer& members,
		const std::vector<std::string>& member_names,
		const std::vector<AttributeContainer>& member_attr) { InsertType(name, qs_file_name, attr_cont, def_value, members, member_names, member_attr, m_TypeMap); }

	bool IsValidTypeInstanceDefinition(const TypeInstanceDescription& instance_desc) const;
	bool IsValidTypeInstanceDefinition(const HashString& type_name) const;
	TypeDefinitionHandle FindHandleFor(const TypeInstanceDescription& instance_desc) const;
	TypeDefinitionHandle FindHandleFor(const HashString& type_name) const;

	TypeMap m_TypeMap;
	QuickScriptInitParams m_InitParams;
};

