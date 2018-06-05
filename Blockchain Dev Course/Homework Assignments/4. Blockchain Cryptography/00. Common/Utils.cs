using System;
using System.Text;

namespace Common
{
    public static class Utils
    {
        public static byte[] GetBytes(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }

        public static string ToString(byte[] value)
        {
            return BitConverter.ToString(value)
                .Replace("-", string.Empty)
                .ToLower();
        }
    }
}
