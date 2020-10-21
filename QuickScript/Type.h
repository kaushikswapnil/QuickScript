#pragma once
#include <vector>
#include <string>
#include "Attribute.h"

struct Object
{
	inline const bool HasMembers() const { return !(Members.empty()); }
	inline const std::vector<Object>& GetMembers() const { return Members; }
	inline std::vector<Object>& GetMembers() { return Members; }

	inline const std::string& GetTypeName() const { return TypeName; }
	inline void SetTypeName(const std::string& value) { TypeName = value; }

	inline const std::string& GetTypeValue() const { return TypeValue; }
	inline void SetTypeValue(const std::string& value) { TypeValue = value; }

	inline const uint8_t GetTypeDescriptionID() const { return TypeDescriptionID; }
	inline void SetTypeDescriptionID(uint8_t val) { TypeDescriptionID = val; }

	std::string TypeName, TypeValue;

	std::vector<Attribute> Attributes;
	std::vector<Object> Members;

	uint8_t TypeDescriptionID { UINT8_MAX };
};

