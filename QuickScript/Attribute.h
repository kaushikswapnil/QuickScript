#pragma once
#include <string>
#include "HashString.h"
#include <vector>
#include <boost/serialization/vector.hpp>

struct Value
{
	HashString ValueString{};

	bool IsValid() const { return ValueString.AsString().size() > 0; }

	template<class Archive>
	void serialize(Archive& ar, const unsigned int version)
	{
		ar & ValueString;
	}
};

struct Attribute
{
	Attribute(const HashString& name) : m_Name(name) {}
	HashString m_Name{};
	std::vector<Value> m_Values{};

	Attribute() = default;
	bool IsValid() const { return true; }

	template<class Archive>
	void serialize(Archive& ar, const unsigned int version)
	{
		ar & m_Name;
		ar & m_Values;
	}
};

