#pragma once

#include <string>

class HashString
{
public:
	typedef unsigned int HashValueType;
	static constexpr HashValueType InvalidValue = 0xdeadbeef;

	HashString();
	HashString(const std::string& value) {}
	HashString(const HashValueType& value) {}
	~HashString();

	const HashValueType AsValue() const;
	std::string AsString() const;

	void Reset() {}
	void Set(const std::string& val) {}
	void Set(const HashValueType val) {}

private:
	std::string m_Name;
	HashValueType m_Hash;
};
