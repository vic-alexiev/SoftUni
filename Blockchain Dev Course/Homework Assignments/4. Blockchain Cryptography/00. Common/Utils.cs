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

        public static byte[] GetHexStringBytes(string hexValue)
        {
            int bytesCount = hexValue.Length / 2;
            byte[] bytes = new byte[bytesCount];
            string hex;
            int j = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                hex = new String(new Char[] { hexValue[j], hexValue[j + 1] });
                bytes[i] = HexToByte(hex);
                j = j + 2;
            }
            return bytes;
        }

        private static byte HexToByte(string hex)
        {
            return byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
        }
    }
}
