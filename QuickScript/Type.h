#pragma once
#include <vector>
#include <string>
#include "Attribute.h"
#include <optional>
#include "HashString.h"

struct Member;

typedef std::vector<Attribute> AttributeContainer;
typedef std::vector<Member> MemberContainer;

struct Value
{
	std::string ValueString{};

	bool IsValid() const { return ValueString.size() > 0;}
};

struct Member
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
	MemberContainer m_Members{};
	HashString m_Name;
	HashString m_Filename;
	bool IsValid() const { return true; }

	void Dump() const;
};

typedef size_t TypeDefinitionHandle;
struct TypeDefinition
{
	HashString m_Name;
	HashString m_QSFileName;
	AttributeContainer m_Attributes;
	std::optional<Value> m_DefaultValue{};
	std::vector<TypeDefinitionHandle> m_Members;
	std::vector<AttributeContainer> m_MemberAttributes;
};

struct TypeMap
{
	std::vector<TypeDefinition> m_Definitions;
};