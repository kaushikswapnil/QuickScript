#pragma once
#include <string>
#include "HashString.h"
#include <vector>
#include <boost/serialization/vector.hpp>

struct Value
{
	HashString ValueString{};

	bool IsValid() const { return ValueString.IsValid(); }

	template<class Archive>
	void serialize(Archive& ar, const unsigned int version)
	{
		ar & ValueString;
	}
};

struct AttributeTag
{
	AttributeTag(const HashString& name) : m_Name(name) {}
	HashString m_Name{};
	std::vector<Value> m_Values{};

	AttributeTag() = default;
	bool IsValid() const { return true; }

	template<class Archive>
	void serialize(Archive& ar, const unsigned int version)
	{
		ar & m_Name;
		ar & m_Values;
	}
};

struct Attribute
{
	struct Flags
	{
		unsigned int IsPrimitiveApplicable : 1;
		unsigned int IsComplexApplicable : 1;
		unsigned int IsGroupable : 1;
	} m_Flags;

	bool IsApplicableValueSet(const std::vector<Value>& value_set) const { return true; }
};

