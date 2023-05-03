using QuickScript.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuickScript
{
    public class DataMap
    {
        public List<AttributeDefinition> AttributeDefinitions = new List<AttributeDefinition>();
        public List<TypeDefinition> TypeDefinitions = new List<TypeDefinition>();

        private DataMap() 
        {
            ConstructBaseMap();
        }

        private static JsonSerializerOptions GetSerializationOptions()
        {
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            return options;
        }

        public static DataMap ConstructDataMap(in string existing_map_file_path = "")
        {
            DataMap retval = null;
            try
            {
                if (existing_map_file_path.Length > 0)
                {
                    if (File.Exists(existing_map_file_path))
                    {
                        DataMap deserialized = JsonSerializer.Deserialize<DataMap>(File.ReadAllText(existing_map_file_path), GetSerializationOptions());
                        if (deserialized != null)
                        {
                            retval = deserialized;
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException(existing_map_file_path);
                    }
                }
                else
                {
                    throw new ArgumentNullException("Empty data map file path");
                }
            }
            catch (Exception ex)
            {
                retval = new DataMap();
            }

            return retval;
        }

        private void ConstructBaseMap()
        {
            ConstructBaseAttributes();
            ConstructBaseTypeDefinitions();
        }

        public void SaveToDisk(in string output_file_path)
        {
            string jsonString = JsonSerializer.Serialize<DataMap>(this);
            File.WriteAllText(output_file_path, jsonString);
        }

        private void ConstructBaseAttributes()
        {
            {
                //Filepath
                AttributeDefinition file_path = new AttributeDefinition(new HashString("FilePath"), 1, 1);
                AttributeDefinitions.Add(file_path); 
            }
        }

        private void ConstructBaseTypeDefinitions()
        {
            {
                //bool
                TypeDefinition boolean = new TypeDefinition(new HashString("bool"));
                TypeDefinitions.Add(boolean);
            }

            {
                //int
                TypeDefinition integer = new TypeDefinition(new HashString("int"));
                TypeDefinitions.Add(integer);
            }

            {
                //char
                TypeDefinition character = new TypeDefinition(new HashString("char"));
                TypeDefinitions.Add(character);
            }

            {
                //float
                TypeDefinition floating = new TypeDefinition(new HashString("float"));
                TypeDefinitions.Add(floating);
            }

            {
                //string
                TypeDefinition str = new TypeDefinition(new HashString("string"));
                TypeDefinitions.Add(str);
            }
        }
    }
}
