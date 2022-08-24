#include "pch.h"
#include "HashString.h"

void HashString::Set(const std::string& val)
{
	Set(*this, val);
}

void HashString::Set(HashString& hash_str, const std::string& val)
{
	static std::hash<std::string> hasher;
	hash_str.m_Str = val;
	hash_str.m_Val = hasher(val);
}
