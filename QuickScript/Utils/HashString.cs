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

        private string m_Str;
        private HashValueType m_Hash;

        public readonly HashValueType InvalidHashValue = new HashValueType();
        public readonly HashString InvalidHashString = new HashString();

        private static Dictionary<HashValueType, string> m_HashedStringDictionary = new Dictionary<HashValueType, string>();

        public HashString()
        {
            this.m_Str = "";
            this.m_Hash = InvalidHashValue;
        }

        public HashString(in string str)
        {
            this.m_Str = str;
            m_Hash = Hash(str);
        }

        public HashString(in string str, in HashValueType hash)
        {
            m_Str = str;
            m_Hash = hash;
        }

        public static bool VerifyUniqueHash(in string str, in HashValueType hash)
        {
            if (m_HashedStringDictionary.ContainsKey(hash))
            {
                return str == m_HashedStringDictionary[hash];
            }

            m_HashedStringDictionary[hash] = str;
            return true;
        }
        public static HashValueType Hash(in string str)
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
    }
}
