#pragma once
#include <filesystem>
class QSParser
{
public:
	static void ParseFile(const std::filesystem::directory_entry& entry);
	static void ExtractType(const std::vector<std::string>& equation_nodes);

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

