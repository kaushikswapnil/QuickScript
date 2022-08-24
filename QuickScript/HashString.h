#pragma once

#include <string>

class HashString
{
public:
	typedef unsigned int HashValueType;

	HashString() = default;
	HashString(const std::string& value) { Set(value);}

	operator std::string() const { return m_Str; }
	operator HashValueType() const { return m_Val;}

	bool operator==(const HashString& rhs) const { return m_Val == rhs.m_Val;}

	const HashValueType AsValue() const;
	std::string AsString() const { return m_Str;}

	void Reset() {}
	void Set(const std::string& val);

	bool IsValid() const { return m_Str.size() > 0; }

private:
	static void Set(HashString& str, const std::string& val);

	std::string m_Str{};
	HashValueType m_Val;
};
