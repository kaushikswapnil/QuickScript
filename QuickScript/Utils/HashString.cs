using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace QuickScript.Utils
{
    public class HashString
    {
        private class HashValueType
        {
            public uint Val;

            public HashValueType(uint val)
            {
                Val = val;
            }
        }

        private string m_Str;
        private HashValueType m_Hash;

        public const HashValueType

        public const HashString InvalidHashString;

        public HashString()
        {

        }

        private static HashValueType Hash(string str)
        {
            using (var hasher = System.Security.Cryptography.MD5.Create())
            {
                var hashed = hasher.ComputeHash(System.Text.Encoding.ASCII.GetBytes(str));
                var ivalue = BitConverter.ToUInt32(hashed, 0);
                return new HashValueType(ivalue);
            }
        }
    }
}
