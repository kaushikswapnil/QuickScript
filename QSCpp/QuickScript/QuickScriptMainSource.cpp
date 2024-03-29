// QuickScript.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <filesystem>
#include "QuickScript.h"

int main()
{
	const std::string main_file_dir{__FILE__};
	std::filesystem::path working_folder = (std::filesystem::path(main_file_dir)).parent_path();
	working_folder.append("ReadDirectory\\");
	QuickScriptInitParams params;
	params.ReadDirectoryPath = working_folder;
	params.TypeMapDirectory = working_folder;
	params.OutputDirectory = working_folder.parent_path();
	params.OutputDirectory /= "Output";
	QuickScript qs{ params };

	return 0;
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
