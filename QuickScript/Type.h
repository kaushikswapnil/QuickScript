#pragma once
#include <vector>
#include <string>
#include "Attribute.h"
#include <optional>
#include "HashString.h"

struct TypeInstanceDescription;

typedef std::vector<Attribute> AttributeContainer;
typedef std::vector<TypeInstanceDescription> MemberContainer;

struct Value
{
	std::string ValueString{};
};

/*
An object is an instance of well an object, it could be used to describe an integer
in another complex class with a value and default attributes
*/
struct TypeInstanceDescription
{
	std::optional<Value> m_DefaultValue{};
	AttributeContainer m_Attributes{};
	MemberContainer m_Members{};
	HashString m_Name;
	HashString m_Filename;
};
