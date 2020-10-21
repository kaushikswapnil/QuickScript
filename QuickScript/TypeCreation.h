#pragma once
#include <vector>
#include <string>
#include "Type.h"

struct ObjectTypeDescription
{
	std::vector<std::string> TypeNames{ };
	bool CanHaveMembers{false};
};

class ObjectFactory
{
public:
	ObjectFactory() = default;
	~ObjectFactory() = default;

	void Initialize(const std::string& object_description_settings_file_path);

	void ParseFile(const std::string& file_to_read, std::vector<Object>& out_types);

private:
	std::vector<ObjectTypeDescription> m_ObjectTypeDescriptions;
};

