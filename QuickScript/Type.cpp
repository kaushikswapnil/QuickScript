#include "pch.h"
#include "Type.h"
#include <iostream>

void TypeInstanceDescription::Dump() const
{
	std::cout << std::endl << "[ ";
	for (const auto& attr : m_Attributes)
	{
		std::cout << attr.m_Name.AsString() << " ";
	}
	std::cout << "]";
	std::cout << std::endl << m_Name.AsString() << "\n{\n";
	for (const auto& member : m_Members)
	{
		std::cout << member.m_TypeName.AsString() << " " 
		<< member.m_Name.AsString();
		if (member.m_DefaultValue.IsValid())
		{
			std::cout << " = " << member.m_DefaultValue.ValueString;
		}
		std::cout << std::endl;
	}
	std::cout << "}";
}
