#pragma once
#include "QSExporter.h"

struct CppQSExporter : public QSExporter
{
	void ExportTypeMap(const TypeMap& type_map, const std::filesystem::path& read_directory, const std::filesystem::path& output_directory) override;
};

