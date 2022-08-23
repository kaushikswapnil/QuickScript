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
#include <fstream>

enum class ReadTypeState {
	Invalid,
	Attribute,
	Class,
	Member,
	Value,
	Count
};

void FetchTypesFromFile(const std::filesystem::directory_entry& entry)
{
	std::ifstream file;
	file.open(entry, std::ios::in);

	char c;
	std::vector<std::string> prev_words;
	std::vector<ReadTypeState> read_state_stack;
	read_state_stack.push_back(ReadTypeState::Invalid);
	std::string cur_word;

	TypeInstanceDescription tentative_desc;
	while (file.get(c))
	{
		ReadTypeState cur_state = read_state_stack.back();
		switch (c)
		{
		case ' ':
			prev_words.push_back(cur_word);
			cur_word.clear();
			break;
		case '{': 
		{
			std::string class_name = prev_words.back();
			prev_words.pop_back();
			std::string struct_or_class_name = prev_words.back();
			prev_words.pop_back();
			read_state_stack.push_back(ReadTypeState::Class);
			tentative_desc.m_Name.Set(class_name);
		}
			break;
		case '}':
		{
			ReadTypeState state = read_state_stack.back();
			read_state_stack.pop_back();
		}
			break;
		case '[':
			read_state_stack.push_back(ReadTypeState::Attribute);
			break;
		case ']':
		{
			ReadTypeState state = read_state_stack.back();
			//std::cout << "Popping state " << state;
			read_state_stack.pop_back();
		}
			break;
		case ',':
			prev_words.push_back(cur_word);
			cur_word.clear();
			break;
		default:
			cur_word.push_back(c);
			break;
		}
	}
}

int main()
{
	const std::string main_file_dir{__FILE__};
	std::filesystem::path working_folder = (std::filesystem::path(main_file_dir)).parent_path();
	working_folder.append("ReadDirectory\\");
	_chdir((working_folder.u8string().c_str()));

	for (const auto& entry : std::filesystem::directory_iterator(working_folder))
	{
		FetchTypesFromFile(entry);
	}

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
