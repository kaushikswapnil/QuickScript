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

typedef std::vector<Value> AttributeValueContainer;

struct AttributeTag
{
	AttributeTag(const HashString& name) : m_Name(name) {}
	HashString m_Name{};
	AttributeValueContainer m_Values{};

	AttributeTag() = default;
	bool IsValid() const { return true; }

	template<class Archive>
	void serialize(Archive& ar, const unsigned int version)
	{
		ar & m_Name;
		ar & m_Values;
	}
};

struct AttributeDefinition
{
	struct Flags
	{
		unsigned int IsPrimitiveApplicable : 1 = 1;
		unsigned int IsComplexApplicable : 1 = 1;
		unsigned int IsGroupable : 1 = 0;
	} m_Flags;

	HashString m_Name;
	uint8_t m_MaximumValueCount;
	uint8_t m_MinimumValueCount;

	bool IsApplicableValueSet(const std::vector<Value>& value_set) const { return true; }
};

