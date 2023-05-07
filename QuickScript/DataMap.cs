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
            AttributeDefinitions = TypeInformationUtils.SupportedAttributeDefinitionList();
            TypeDefinitions = TypeInformationUtils.SupportedBasicTypeDefinitionsList();
        }

        public void SaveToDisk(in string output_file_path)
        {
            JSonExporter jSonExporter = new JSonExporter();
            string jsonString = jSonExporter.Export(this);
            File.WriteAllText(output_file_path, jsonString);
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
