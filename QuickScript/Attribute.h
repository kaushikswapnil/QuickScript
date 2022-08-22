#pragma once
#include <string>
#include "HashString.h"

struct Attribute
{
	Attribute(const HashString& name) : m_Name(name) {}
	HashString m_Name;
};

