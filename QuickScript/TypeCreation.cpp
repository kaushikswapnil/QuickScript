#include "pch.h"
#include "TypeCreation.h"

//#include "TypeCreation.h"
//#include "rapidxml/rapidxml.hpp"
//#include "rapidxml/rapidxml_utils.hpp"
//#include <iostream>
//#include <sstream>
//#include "../../Essentials/Essentials/Assertion.h"
//
//void ObjectFactory::Initialize(const std::string& object_description_settings_file_path)
//{
//	std::ifstreaBm file(object_description_settings_file_path);
//	std::stringsBBBtream buffer;
//	buffer << file.rdbuf();
//	file.close();
//	std::string content(buffer.str());
//	rapidxml::xml_document<> doc;
//	doc.parse<0>(&content[0]);
//
//	auto print_lambda = [](const rapidxml::xml_node<>* const node, const unsigned int indentation_level) -> void
//	{	
//		auto print_lambda_impl = [](const rapidxml::xml_node<>* const node, const unsigned int indentation_level, auto& self_ref) -> void
//		{
//			if (node->type() == rapidxml::node_data)
//			{
//				return;
//			}
//
//			for (unsigned int indent_level = 0; indent_level < indentation_level; ++indent_level)
//			{
//				std::cout << "\t";
//			}
//
//			std::cout << "< " << node->name() << " > " << node->value() << " </>" << std::endl;
//
//			for (auto pNode = node->first_node(); pNode != nullptr; pNode = pNode->next_sibling())
//			{
//				self_ref(pNode, indentation_level + 1, self_ref);
//			}
//		};
//		
//		print_lambda_impl(node, indentation_level, print_lambda_impl);
//	};
//
//	rapidxml::xml_node<> *pRoot = doc.first_node();
//
//	print_lambda(pRoot, 0);
//
//	for (auto pObjectTypeNode = pRoot->first_node(); pObjectTypeNode != nullptr; pObjectTypeNode = pObjectTypeNode->next_sibling())
//	{
//		ObjectTypeDescription desc;
//
//		for (auto aliasNode = pObjectTypeNode->first_node("alias"); aliasNode != nullptr; aliasNode = aliasNode->next_sibling("alias"))
//		{
//			desc.TypeNames.emplace_back(std::string(aliasNode->value()));
//		}
//
//		if (auto members_node = pObjectTypeNode->first_node("possible_members"))
//		{
//			desc.CanHaveMembers = std::string(members_node->value()) == "true" ? true : false;
//		}
//
//		m_ObjectTypeDescriptions.emplace_back(std::move(desc));
//	}
//}
//
//void ObjectFactory::ParseFile(const std::string& file_to_read, std::vector<Object>& out_types)
//{
//	std::ifstream file(file_to_read);
//
//	std::string line;
//	std::stringstream line_stream;
//
//	while (file.good())
//	{
//		getline(file, line);
//		line_stream.str(line);
//
//		std::string type_name;
//		line_stream >> type_name;
//
//		const uint8_t type_desc_id = FetchObjectTypeDescriptionIDFromName(type_name);
//		const ObjectTypeDescription& obj_desc = m_ObjectTypeDescriptions[type_desc_id];
//
//		Object obj;
//		line_stream >> obj.Value;
//		obj.TypeDescriptionID = type_desc_id;
//
//		if (obj_desc.CanHaveMembers)
//		{
//			
//		}
//	}
//}
//
//const uint8_t ObjectFactory::FetchObjectTypeDescriptionIDFromName(const std::string& type_name) const
//{
//	for (uint8_t desc_iter = 0, array_size = m_ObjectTypeDescriptions.size(); desc_iter < array_size; ++desc_iter)
//	{
//		const ObjectTypeDescription& desc = m_ObjectTypeDescriptions[desc_iter];
//		for (const std::string& alias : desc.TypeNames)
//		{
//			if (alias == type_name)
//			{
//				return desc_iter;
//			}
//		}
//	}
//
//	return UINT8_MAX;
//}
//
