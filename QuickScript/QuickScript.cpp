// QuickScript.cpp : This file contains the 'main' function. Program execution begins and ends there.
//

#include "pch.h"
#include <iostream>
#include <string>
#include <filesystem>
#include <direct.h>
#include "Type.h"
#include "TypeCreation.h"
#include "TypeSerialization.h"

int main()
{
	const std::string working_dir{__FILE__};
	_chdir((((std::filesystem::path(working_dir)).parent_path()).u8string()).c_str());

	std::string file_path{"PrimitiveObjectTypeDescriptions.xml"};
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
