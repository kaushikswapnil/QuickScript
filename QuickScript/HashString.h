#pragma once

#include <string>

class HashString
{
public:
	typedef unsigned int HashValueType;
	static constexpr HashValueType InvalidValue = 0xdeadbeef;

	HashString() = default;
	HashString(const std::string& value) { Set(value);}

	const HashValueType AsValue() const;
	std::string AsString() const { return m_Str;}

	void Reset() {}
	void Set(const std::string& val) { m_Str = val;}

	bool IsValid() const { return m_Str.size() > 0; }

private:
	std::string m_Str{};
	HashValueType m_Val;
};
