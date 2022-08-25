#pragma once

#include <string>

class HashString
{
public:
	typedef size_t HashValueType;

	HashString() { Reset(); }
	HashString(const std::string& value) { Set(value);}

	operator std::string() const { return m_Str; }
	operator HashValueType() const { return m_Val;}

	bool operator==(const HashString& rhs) const { return m_Val == rhs.m_Val;}

	const HashValueType AsValue() const { return m_Val;}
	std::string AsString() const { return m_Str;}

	void Reset();
	void Set(const std::string& val);

	bool IsValid() const { return *this != InvalidHashString(); }

	static const HashString& InvalidHashString();

private:
	static HashValueType GetHashFor(const std::string& val);

	std::string m_Str{};
	HashValueType m_Val;
};

namespace std {

	template <>
	struct hash<HashString>
	{
		std::size_t operator()(const HashString& k) const
		{
			return k.AsValue();
		}
	};

}
