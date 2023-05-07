using QuickScript.Exporters;
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

        public DataMap(List<AttributeDefinition> attributeDefinitions, List<TypeDefinition> typeDefinitions)
        {
            AttributeDefinitions = attributeDefinitions;
            TypeDefinitions = typeDefinitions;
        }

        public DataMap() 
        {
            ConstructBaseMap();
        }

        public static DataMap ConstructDataMap(in string existing_map_file_path = "", bool force_recreate_base = false)
        {
            DataMap retval = null;
            if (force_recreate_base == false)
            {
                try
                {
                    if (existing_map_file_path.Length > 0)
                    {
                        if (File.Exists(existing_map_file_path))
                        {
                            DataMap deserialized = JsonSerializer.Deserialize<DataMap>(File.ReadAllText(existing_map_file_path), JSonExportUtils.GetSerializationOptions());
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
                    Assertion.Warn("Unable to create datamap from existing file!");
                    Assertion.SoftAssert(false, ex.Message);
                    retval = new DataMap(new List<AttributeDefinition>(), new List<TypeDefinition>());
                    retval.ConstructBaseMap();
                }
            }
            else
            {
                retval = new DataMap(new List<AttributeDefinition>(), new List<TypeDefinition>());
                retval.ConstructBaseMap();
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
            JSonExporter jSonExporter = new JSonExporter();
            string jsonString = jSonExporter.Export(this);
            File.WriteAllText(output_file_path, jsonString);
        }

        private void ConstructBaseAttributes()
        {
            {
                //Filepath
                AttributeDefinition file_path = new AttributeDefinition(new HashString("FilePath"), 1, new HashString("string"));
                AttributeDefinitions.Add(file_path); 
            }
            {
                //Alias
                AttributeDefinition alias = new AttributeDefinition(new HashString("Alias"), 1, -1, new HashString("string"));
                AttributeDefinitions.Add(alias);
            }
            {
                //DefaultValue
                AttributeDefinition def_val = new AttributeDefinition(new HashString("DefaultValue"), 0, 1, new HashString("string"));
                AttributeDefinitions.Add(def_val);
            }
            {
                //Precision
                AttributeDefinition def_val = new AttributeDefinition(new HashString("Precision"), 0, 1, new HashString("int"));
                AttributeDefinitions.Add(def_val);
            }
        }

        private void ConstructBaseTypeDefinitions()
        {
            {
                //bool
                TypeDefinition boolean = new TypeDefinition { Name = new HashString("bool"),
                    Attributes = new List<AttributeTag> { new AttributeTag(new HashString("DefaultValue"), new List<ValueType> { new ValueType("true") }) }
                };
                TypeDefinitions.Add(boolean);
            }

            {
                //int
                TypeDefinition integer = new TypeDefinition
                {
                    Name = new HashString("int"),
                    Attributes = new List<AttributeTag> { new AttributeTag(new HashString("DefaultValue"), new List<ValueType> { new ValueType("0") }) }
                };
                TypeDefinitions.Add(integer);
            }

            {
                //char
                TypeDefinition character = new TypeDefinition
                {
                    Name = new HashString("char"),
                    Attributes = new List<AttributeTag> { new AttributeTag(new HashString("DefaultValue"), new List<ValueType> { new ValueType(" ") }) }
                };
                TypeDefinitions.Add(character);
            }

            {
                //float
                TypeDefinition floating = new TypeDefinition
                {
                    Name = new HashString("float"),
                    Attributes = new List<AttributeTag> { new AttributeTag(new HashString("DefaultValue"), new List<ValueType> { new ValueType("0.0") }) }
                };
                TypeDefinitions.Add(floating);
            }

            {
                //string
                TypeDefinition str = new TypeDefinition
                {
                    Name = new HashString("string"),
                    Attributes = new List<AttributeTag> { new AttributeTag(new HashString("DefaultValue"), new List<ValueType> { new ValueType("") }) }
                };
                TypeDefinitions.Add(str);
            }
        }

        public override int GetHashCode()
        {
            return AttributeDefinitions.GetHashCode() ^ TypeDefinitions.GetHashCode();
        }
        public override bool Equals(object o)
        {
            if (!(o is DataMap))
                return false;
            
            DataMap x = (DataMap)o;
            DataMap y = this;

            return x.AttributeDefinitions.SequenceEqual(y.AttributeDefinitions) &&
                   x.TypeDefinitions.SequenceEqual(y.TypeDefinitions);
        }

        public TypeDefinition? GetTypeDefinitionByName(HashString name)
        {
            return TypeDefinitions.Find(x => x.Name == name);
        }

        public bool HasTypeDefinitionByName(HashString name)
        {
            return GetTypeDefinitionByName(name) != null;
        }

        public AttributeDefinition? GetAttributeDefinitionByName(HashString name)
        {
            return AttributeDefinitions.Find(x => x.Name == name);
        }

        public bool HasAttributeDefinitionByName(HashString name)
        {
            return GetAttributeDefinitionByName(name) != null;
        }

        public void AssimilateTypeInstanceDescriptions(in List<TypeInstanceDescription> typeInstanceDescriptions)
        {
            List<TypeDefinition> processed_defs = new List<TypeDefinition>();

            foreach (TypeInstanceDescription description in typeInstanceDescriptions)
            {
                TypeDefinition? new_def = TypeInformationUtils.ParseTypeDescriptionToPotentialDefinition(this, description);

                if (new_def != null)
                {
                    TypeDefinition? existing_def = GetTypeDefinitionByName(description.Name);

                    if (existing_def != null)
                    {
                        //we have a type with this name already
                        //check for any changes
                        //throw errors
                    }
                    else
                    {
                        processed_defs.Add(new_def);
                    }
                }
            }

            TypeDefinitions.AddRange(processed_defs);
        }
    }
}
