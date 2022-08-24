#include "pch.h"
#include "Type.h"
#include <iostream>

void TypeInstanceDescription::Dump() const
{
	if (m_Attributes.size() > 0)
	{
		std::cout << std::endl << "[ ";
		for (const auto& attr : m_Attributes)
		{
			std::cout << attr.m_Name.AsString() << " ";
		}
		std::cout << "]";
	}
	
	std::cout << std::endl << m_Name.AsString() << "\n{\n";
	for (const auto& member : m_Members)
	{
		if (member.m_Attributes.size() > 0)
		{
			std::cout << "[";
			for (const auto& attr : member.m_Attributes)
			{
				std::cout << attr.m_Name.AsString() << " ";
			}
			std::cout << "]" << std::endl;
		}
		
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
