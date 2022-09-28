#include "QSExtractor.h"
#include <fstream>

void QSExtractor::OutputTypeMapAsCpp(const TypeMap& type_map, const std::filesystem::path& read_directory, const std::filesystem::path& output_directory)
{
	for (const auto& entry : type_map.m_Definitions)
	{
		if (!entry.IsPrimitive())
		{
			std::filesystem::path rel_path = std::filesystem::relative(std::filesystem::path(entry.m_QSFileName), read_directory);
			std::filesystem::path cpp_file_path = output_directory;
			cpp_file_path /= rel_path.parent_path();

			//currently it points to the parent folder
			if (!std::filesystem::exists(cpp_file_path))
			{
				std::filesystem::create_directory(cpp_file_path);
			}
			cpp_file_path /= entry.m_Name.AsString();
			cpp_file_path.replace_extension(".h");

			std::ofstream cpp_file{ cpp_file_path, std::ofstream::out | std::ofstream::binary };
			static const std::string qs_gen_area{"QS_GENERATED_AREA"};
			static const std::string qs_user_area{ "QS_USER_AREA" };

			cpp_file << qs_gen_area << std::endl;
			cpp_file << "class " << entry.m_Name.AsString() << std::endl << "{\n";
			for (auto member_iter = 0; member_iter < entry.m_Members.size(); ++ member_iter)
			{
				const auto member_handle = entry.m_Members[member_iter];
				cpp_file << '\t' << type_map.m_Definitions[member_handle].m_Name.AsString() << ' ' << entry.m_MemberNames[member_iter] << ";\n";
			}
			cpp_file << "}\n" << qs_gen_area;
			cpp_file.close();
		}
	}
}
