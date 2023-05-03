using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Utils
{
    public class HashString
    {
        public class HashValueType
        {
            public uint Val;
            public const uint InvalidHashValue = uint.MaxValue;

            public HashValueType(uint val)
            {
                Val = val;
            }

            public HashValueType()
            {
                Val = InvalidHashValue;
            }

            public static implicit operator uint(HashValueType hash) => hash.Val;
            public static explicit operator HashValueType(uint val) => new HashValueType(val);
        }

        private string Str { get; set; }
        private HashValueType Hash { get; set; }

        public readonly static HashValueType InvalidHashValue = new HashValueType();
        public readonly static HashString InvalidHashString = new HashString();

        private static Dictionary<HashValueType, string> CreateHashedStringDictionary()
        {
            var ret_val = new Dictionary<HashValueType, string>();
            ret_val[InvalidHashValue] = "Invalid";
            return ret_val;
        }

        private static Dictionary<HashValueType, string> HashedStringDictionary = CreateHashedStringDictionary();

        public HashString()
        {
            this.Str = "";
            this.Hash = InvalidHashValue;
        }

        public HashString(in string str)
        {
            this.Str = str;
            this.Hash = GenerateHash(str);
        }

        public HashString(in string str, in HashValueType hash)
        {
            Str = str;
            Hash = hash;
            VerifyUniqueHash(str, hash);
        }

        public void Reset(in string str)
        {
            Hash = GenerateHash(str);
            Str = str;
        }

        public void Reset(in HashValueType hash)
        {
            Hash = hash;
            Str = GetStringFromHash(hash);
        }
        public void Reset(in string str, in HashValueType hash)
        {
            Str = str; Hash = hash;
            VerifyUniqueHash(str, hash);
        }

        public bool IsValid()
        {
            return Str.Length > 0 || Hash != InvalidHashValue;
        }

        private string GetStringFromHash(in HashValueType hash)
        {
            if (HashedStringDictionary.ContainsKey(hash) == false)
            {
                return "Not Found";
            }

            return HashedStringDictionary[hash];
        }
        public static bool VerifyUniqueHash(in string str, in HashValueType hash)
        {
            if (HashedStringDictionary.ContainsKey(hash))
            {
                return str == HashedStringDictionary[hash];
            }

            HashedStringDictionary[hash] = str;
            return true;
        }
        public static HashValueType GenerateHash(in string str)
        {
            using (var hasher = System.Security.Cryptography.MD5.Create())
            {
                var hashed = hasher.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
                var ivalue = BitConverter.ToUInt32(hashed, 0);
                var ret_val = new HashValueType(ivalue);

                VerifyUniqueHash(str, ret_val);

                return ret_val;
            }
        }

        public string AsString()
        {
            return Str;
        }
    }
}
