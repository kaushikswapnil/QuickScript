#pragma once
#include <vector>
#include <string>
#include "Attribute.h"
#include <optional>
#include "HashString.h"
#include <unordered_map>
#include <boost/serialization/vector.hpp>
#include <boost/serialization/unordered_map.hpp>

struct TypeInstanceMember;

typedef std::vector<AttributeTag> AttributeTagContainer;
typedef std::vector<TypeInstanceMember> TypeInstanceMemberContainer;

//describes a member of a type
struct TypeInstanceMember
{
	HashString m_Name{};
	HashString	m_TypeName{};
	AttributeTagContainer m_Attributes{};
	Value m_DefaultValue{};
};

//describes a type, but isnt validated or concrete yet.
struct TypeInstanceDescription
{
	std::optional<Value> m_DefaultValue{};
	AttributeTagContainer m_Attributes{};
	TypeInstanceMemberContainer m_Members{};
	HashString m_Name;
	HashString m_Filename;
	bool IsValid() const { return true; }

	void Dump() const;

	void Reset();
};

typedef uint32_t TypeDefinitionHandle;
constexpr TypeDefinitionHandle INVALID_TYPE_DEFINITION_HANDLE = UINT32_MAX;
typedef std::vector<TypeDefinitionHandle> TypeDefinitionHandleContainer;

typedef std::vector<HashString> TypeDefinitionMemberNamesContainer;
typedef std::vector<AttributeTagContainer> TypeDefinitionMemberAttributesContainer;

typedef uint32_t AttributeDefinitionHandle;
const AttributeDefinitionHandle INVALID_ATTRIBUTE_DEFINITION_HANDLE = UINT32_MAX;

struct AttributeReference
{
	AttributeDefinitionHandle m_DefinitionHandle;

};

//an actual valid type definition
struct TypeDefinition
{
	HashString m_Name{};
	std::string m_QSFileName{};
	AttributeTagContainer m_Attributes{};
	TypeDefinitionHandleContainer m_Members{};
	TypeDefinitionMemberNamesContainer m_MemberNames{};
	TypeDefinitionMemberAttributesContainer m_MemberAttributes{};

	TypeDefinition() = default;

	TypeDefinition(const std::string& name,
		const std::string qs_file,
		const AttributeTagContainer& attr,
		const TypeDefinitionHandleContainer& members,
		const TypeDefinitionMemberNamesContainer& member_names,
		const TypeDefinitionMemberAttributesContainer& member_attr) :
			m_Name(name), m_QSFileName(qs_file), m_Attributes(attr),
			m_Members(members), m_MemberNames(member_names), m_MemberAttributes(member_attr) {}

	template<class Archive>
	void serialize(Archive & ar, const unsigned int version)
	{
		ar & m_Name;
		ar & m_QSFileName;
		ar & m_Attributes;
		ar & m_Members;
		ar & m_MemberNames;
		ar & m_MemberAttributes;
	}

	bool IsPrimitive() const { return m_QSFileName.empty(); }
};

struct TypeMap
{
	std::vector<TypeDefinition> m_Definitions;
	std::unordered_map<HashString, TypeDefinitionHandle> m_NameHashToHandleMap;
	std::vector<AttributeDefinition> m_AttributeDefinitions;

	template<class Archive>
	void serialize(Archive & ar, const unsigned int version)
	{
		ar & m_Definitions;
		ar & m_NameHashToHandleMap;
	}
};