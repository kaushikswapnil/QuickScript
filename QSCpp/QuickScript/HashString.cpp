#include "pch.h"
#include "HashString.h"

void HashString::Reset()
{
	m_Str.clear();
	m_Val = GetHashFor(m_Str);
}

void HashString::Set(const std::string& val)
{
	m_Str = val;
	m_Val = GetHashFor(val);
}

const HashString& HashString::InvalidHashString()
{
	static const HashString INVALID{};
	return INVALID;
}

HashString::HashValueType HashString::GetHashFor(const std::string& val)
{
	static std::hash<std::string> hasher;
	return hasher(val);
}
