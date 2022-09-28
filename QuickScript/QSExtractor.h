#pragma once
#include "Type.h"
#include <filesystem>

struct QSExtractor
{
	static void OutputTypeMapAsCpp(const TypeMap& type_map, const std::filesystem::path& read_directory, const std::filesystem::path& output_directory);
};

