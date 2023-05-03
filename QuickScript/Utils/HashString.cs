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

        public HashString(in HashValueType hash)
        {
            Assertion.SoftAssert(ReverseStringExists(hash), "In order to use this, we must have a string to lookup");
            Hash = hash;
            Str = GetStringFromHash(hash);
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
            Assertion.SoftAssert(ReverseStringExists(hash), "In order to use this, we must have a string to lookup");
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

        static public HashString FromString(in string str)
        {
            return new HashString(str);
        }

        private static string FormatStringForHash(in string str)
        {
            return str.ToLower();
        }
        private static bool ReverseStringExists(in HashValueType hash)
        {
            return HashedStringDictionary.ContainsKey(hash);
        }

        private static string GetStringFromHash(in HashValueType hash)
        {
            if (ReverseStringExists(hash) == false)
            {
                return "Not Found";
            }

            return HashedStringDictionary[hash];
        }
        public static bool VerifyUniqueHash(in string str, in HashValueType hash)
        {
            if (ReverseStringExists(hash))
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
                string to_hash = FormatStringForHash(str);
                var hashed = hasher.ComputeHash(System.Text.Encoding.ASCII.GetBytes(to_hash));
                var ivalue = BitConverter.ToUInt32(hashed, 0);
                var ret_val = new HashValueType(ivalue);

                VerifyUniqueHash(to_hash, ret_val);

                return ret_val;
            }
        }

        public string AsString()
        {
            return Str;
        }

        public uint AsHash()
        {
            return Hash;
        }
    }
}
