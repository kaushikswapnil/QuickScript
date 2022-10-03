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

typedef std::vector<AttributeTag> AttributeContainer;
typedef std::vector<TypeInstanceMember> TypeInstanceMemberContainer;

struct TypeInstanceMember
{
	HashString m_Name{};
	HashString	m_TypeName{};
	AttributeContainer m_Attributes{};
	Value m_DefaultValue{};
};

struct TypeInstanceDescription
{
	std::optional<Value> m_DefaultValue{};
	AttributeContainer m_Attributes{};
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
typedef std::vector<AttributeContainer> TypeDefinitionMemberAttributesContainer;

struct TypeDefinition
{
	HashString m_Name{};
	std::string m_QSFileName{};
	AttributeContainer m_Attributes{};
	TypeDefinitionHandleContainer m_Members{};
	TypeDefinitionMemberNamesContainer m_MemberNames{};
	TypeDefinitionMemberAttributesContainer m_MemberAttributes{};

	TypeDefinition() = default;

	TypeDefinition(const std::string& name,
		const std::string qs_file,
		const AttributeContainer& attr,
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

	template<class Archive>
	void serialize(Archive & ar, const unsigned int version)
	{
		ar & m_Definitions;
		ar & m_NameHashToHandleMap;
	}
};