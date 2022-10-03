#pragma once
#include "Type.h"
#include <filesystem>

struct QSExporter
{
	//can be virtual later
	virtual void ExportTypeMap(const TypeMap& type_map, const std::filesystem::path& read_directory, const std::filesystem::path& output_directory) = 0;
};

struct CppQSExporter : public QSExporter
{
	void ExportTypeMap(const TypeMap& type_map, const std::filesystem::path& read_directory, const std::filesystem::path& output_directory) override;
};


