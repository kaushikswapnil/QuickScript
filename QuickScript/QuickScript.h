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
		const AttributeTagContainer& attr_cont,
		const TypeDefinitionHandleContainer& members,
		const TypeDefinitionMemberNamesContainer& member_names,
		const TypeDefinitionMemberAttributesContainer& member_attr,
		TypeMap& out_to_type_map);
	void InsertType(const HashString& name,
		const std::string& qs_file_name,
		const AttributeTagContainer& attr_cont,
		const TypeDefinitionHandleContainer& members,
		const TypeDefinitionMemberNamesContainer& member_names,
		const TypeDefinitionMemberAttributesContainer& member_attr) { InsertType(name, qs_file_name, attr_cont, members, member_names, member_attr, m_TypeMap); }

	bool IsExistingTypeDefinition(const TypeInstanceDescription& instance_desc) const;
	bool IsExistingTypeDefinition(const HashString& type_name) const;
	TypeDefinitionHandle FindHandleFor(const TypeInstanceDescription& instance_desc) const;
	TypeDefinitionHandle FindHandleFor(const HashString& type_name) const;

	bool MeetsSynctacticallyValidNameCriteria(const std::string& name_as_string);
	bool IsSyntacticallyValidTypeName(const HashString& type_name);
	bool IsSyntacticallyValidVariableName(const HashString& var_name);
	bool TypeInstanceDescriptionHasValidSyntax(const TypeInstanceDescription& instance);

	TypeMap m_TypeMap;
	QuickScriptInitParams m_InitParams;
};

