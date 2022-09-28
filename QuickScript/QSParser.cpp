#include "pch.h"
#include "QSParser.h"
#include <iostream>
#include <string>
#include <fstream>
#include "Type.h"
#include <Assertion.h>

void QSParser::ParseFile(const std::filesystem::directory_entry& entry, std::vector<TypeInstanceDescription>& out_extracted_types)
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
		case ';':
		case '{':
		case '}':
		case '[':
		case ']':
			if (cur_word.size() > 0)
			{
				words.push_back(cur_word);
				cur_word.clear();
			}
			words.push_back(std::string{c});
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

	ExtractType(words, out_extracted_types);
}

void QSParser::ExtractType(const std::vector<std::string>& equation_nodes, std::vector<TypeInstanceDescription>& out_extracted_types)
{
	std::vector<ReadTypeState> state_stack;
	state_stack.push_back(ReadTypeState::Class);

	AttributeContainer attributes;
	TypeInstanceMemberContainer members;

	TypeInstanceDescription tentative_type;

	std::vector<TypeInstanceDescription> tentative_types_in_file;

	std::vector<std::string> unhandled_eq_nodes;

	for (const auto& str : equation_nodes)
	{
		ReadTypeState cur_state = state_stack.back();
		ReadTypeState prev_state = state_stack.size() > 1 ? state_stack[state_stack.size() - 2] : ReadTypeState::Invalid;

		if (str == "{")
		{
			const std::string class_name = unhandled_eq_nodes.back();
			unhandled_eq_nodes.pop_back();
			const std::string class_or_struct = unhandled_eq_nodes.back();
			unhandled_eq_nodes.pop_back();
			tentative_type.m_Name.Set(class_name);
			state_stack.push_back(ReadTypeState::Member);
		}
		else if (str == "}")
		{
			tentative_type.m_Members = members;
			HARDASSERT(state_stack.back() == ReadTypeState::Member, "Should be reading a member here");
			state_stack.pop_back();//this should pop back a member;
			tentative_types_in_file.push_back(tentative_type);
			tentative_type.Reset();
		}
		else if (str == "[")
		{
			HARDASSERT(state_stack.back() == ReadTypeState::Member || state_stack.back() == ReadTypeState::Class, "Should be reading member/class here");
			state_stack.push_back(ReadTypeState::Attribute);
		}
		else if (str == "]")
		{
			HARDASSERT(state_stack.back() == ReadTypeState::Attribute, "Should be reading attribtute here");
			state_stack.pop_back();
			if (prev_state == ReadTypeState::Class)
			{
				tentative_type.m_Attributes.swap(attributes);

			}
			else
			{
				//if it is member, then we chill and wait for the member to be read
			}
		}
		else if (str == ";")
		{
			if (cur_state == ReadTypeState::Member)
			{
				HARDASSERT(unhandled_eq_nodes.size() > 1, "We should have atleast the type and name");
				Value default_val;
				if (unhandled_eq_nodes.size() > 2)
				{
					//type, name, value
					default_val.ValueString = unhandled_eq_nodes.back();
					unhandled_eq_nodes.pop_back(); //for the val
					unhandled_eq_nodes.pop_back(); //=
				}
				TypeInstanceMember member;
				member.m_DefaultValue = default_val;
				member.m_Name.Set(unhandled_eq_nodes.back());
				unhandled_eq_nodes.pop_back();
				member.m_TypeName.Set(unhandled_eq_nodes.back());
				unhandled_eq_nodes.pop_back();
				if (attributes.size() > 0)
				{
					member.m_Attributes.swap(attributes);
				}
				members.push_back(member);
			}
		}
		else
		{
			if (cur_state == ReadTypeState::Attribute)
			{
				attributes.emplace_back(str);
			}
			else
			{
				unhandled_eq_nodes.push_back(str);
			}
		}
	}

	for (const auto& t_type : tentative_types_in_file)
	{
		t_type.Dump();
	}

	out_extracted_types.insert(out_extracted_types.end(), tentative_types_in_file.begin(), tentative_types_in_file.end());
}
