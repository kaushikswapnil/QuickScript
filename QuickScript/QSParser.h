#pragma once
#include <filesystem>

struct TypeInstanceDescription;
class QSParser
{
public:
	static void ParseFile(const std::filesystem::directory_entry& entry, std::vector<TypeInstanceDescription>& out_extracted_types);
	static void ExtractType(const std::vector<std::string>& equation_nodes, std::vector<TypeInstanceDescription>& out_extracted_types);

	enum class ReadTypeState
	{
		Invalid,
		Class,
		Attribute,
		Value,
		Member,
		Count
	};
};

