#include "pch.h"
#include "QuickScript.h"
#include "QSParser.h"

QuickScript::QuickScript(const QuickScriptInitParams& params)
{
	//_chdir((params.ReadDirectoryPath.u8string().c_str()));

	for (const auto& entry : std::filesystem::directory_iterator(params.ReadDirectoryPath))
	{
		QSParser::ParseFile(entry);
	}
}
