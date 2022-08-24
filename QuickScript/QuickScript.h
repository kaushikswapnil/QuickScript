#pragma once
#include <filesystem>

struct QuickScriptInitParams
{
	std::filesystem::path ReadDirectoryPath;
};


class QuickScript
{
public:
	QuickScript(const QuickScriptInitParams& params);
	~QuickScript() = default;
};

