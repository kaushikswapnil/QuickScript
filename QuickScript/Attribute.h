#pragma once
#include <string>
#include "HashString.h"
#include <vector>

struct Value
{
	HashString ValueString{};

	bool IsValid() const { return ValueString.AsString().size() > 0; }
};

struct Attribute
{
	Attribute(const HashString& name) : m_Name(name) {}
	HashString m_Name;
	bool IsValid() const { return true;}
	std::vector<Value> m_Values;
};

