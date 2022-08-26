#pragma once
#include <vector>
#include <string>
#include "Attribute.h"
#include <optional>
#include "HashString.h"
#include <unordered_map>

struct TypeInstanceMember;

typedef std::vector<Attribute> AttributeContainer;
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
};

typedef size_t TypeDefinitionHandle;
typedef std::vector<TypeDefinitionHandle> TypeDefinitionHandleContainer;
struct TypeDefinition
{
	HashString m_Name;
	HashString m_QSFileName;
	AttributeContainer m_Attributes;
	Value m_DefaultValue{};
	TypeDefinitionHandleContainer m_Members;
	std::vector<AttributeContainer> m_MemberAttributes;

	TypeDefinition(const std::string& name,
		const std::string qs_file,
		const AttributeContainer& attr,
		const Value& def_val,
		const TypeDefinitionHandleContainer& members,
		const std::vector<AttributeContainer>& member_attr) :
			m_Name(name), m_QSFileName(qs_file), m_Attributes(attr),
			m_DefaultValue(def_val), m_Members(members), m_MemberAttributes(member_attr) {}

	template<class Archive>
	void serialize(Archive & ar, const unsigned int version)
	{
		ar & m_Name;
		ar & m_QSFileName;
		ar & m_Attributes;
		ar & m_DefaultValue;
		ar & m_Members;
		ar & m_MemberAttributes;
	}
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