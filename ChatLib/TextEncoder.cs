using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib
{
    public static class TextEncoder
    {
        static Encoding Format = Encoding.UTF8;

        public static string Decode(byte[] data, int length)
        {
            return Format.GetString(data, 0, length);
        }

        public static byte[] Encode(string message)
        {
            return Format.GetBytes(message);
        }
    }
}
