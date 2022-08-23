#include "pch.h"
#include "QSParser.h"
#include <iostream>
#include <string>
#include <fstream>

void QSParser::ParseFile(const std::filesystem::directory_entry& entry)
{
	std::ifstream file;
	file.open(entry, std::ios::in);

	char c;
	std::vector<std::string> words;
	std::string cur_word;

	while (file.get(c))
	{
	switch (c)
		{
		case ' ':
		case '\n':
		case '\t':
			if (cur_word.size() > 0)
			{
				words.push_back(cur_word);
				cur_word.clear();
			}
			break;
		default:
			cur_word.push_back(c);
			break;
		}
	}
	if (cur_word.size() > 0)
	{
		words.push_back(cur_word);
	}

	ExtractType(words);
}

void QSParser::ExtractType(const std::vector<std::string>& equation_nodes)
{
	for (const auto& str : equation_nodes)
	{
		std::cout << str;
	}
}
