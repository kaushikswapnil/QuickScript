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

        public HashString(string str)
        {
            this.Str = str;
            this.Hash = GenerateHash(str);
        }

        public HashString(HashValueType hash)
        {
            Assertion.SoftAssert(ReverseStringExists(hash), "In order to use this, we must have a string to lookup");
            Hash = hash;
            Str = GetStringFromHash(hash);
        }

        public HashString(string str, HashValueType hash)
        {
            Str = str;
            Hash = hash;
            VerifyUniqueHash(str, hash);
        }

        public static bool operator ==(HashString x, HashString y)
        {
            return x.Hash == y.Hash;
        }
        public static bool operator !=(HashString x, HashString y)
        {
            return x.Hash != y.Hash;
        }
        public override bool Equals(object o)
        {
            if (!(o is HashString))
                return false;
            return this == (HashString)o;
        }

        public void Reset(string str)
        {
            Hash = GenerateHash(str);
            Str = str;
        }

        public void Reset(HashValueType hash)
        {
            Assertion.SoftAssert(ReverseStringExists(hash), "In order to use this, we must have a string to lookup");
            Hash = hash;
            Str = GetStringFromHash(hash);
        }
        public void Reset(string str, HashValueType hash)
        {
            Str = str; Hash = hash;
            VerifyUniqueHash(str, hash);
        }

        public bool IsValid()
        {
            return Str.Length > 0 || Hash != InvalidHashValue;
        }

        static public HashString FromString(string str)
        {
            return new HashString(str);
        }

        private static string FormatStringForHash(string str)
        {
            return str.ToLower();
        }
        private static bool ReverseStringExists(HashValueType hash)
        {
            return HashedStringDictionary.ContainsKey(hash);
        }

        private static string GetStringFromHash(HashValueType hash)
        {
            if (ReverseStringExists(hash) == false)
            {
                return "Not Found";
            }

            return HashedStringDictionary[hash];
        }
        public static bool VerifyUniqueHash(string str, HashValueType hash)
        {
            if (ReverseStringExists(hash))
            {
                return str == HashedStringDictionary[hash];
            }

            HashedStringDictionary[hash] = str;
            return true;
        }
        public static HashValueType GenerateHash(string str)
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
        public override string? ToString()
        {
            return AsString();
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
