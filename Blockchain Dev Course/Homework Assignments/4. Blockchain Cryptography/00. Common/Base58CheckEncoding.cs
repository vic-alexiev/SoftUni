using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;

namespace Common
{
    /// <summary>
    /// Base58Check Encoding / Decoding (Bitcoin-style)
    /// </summary>
    /// <remarks>
    /// See here for more details: 
    /// https://en.bitcoin.it/wiki/Base58Check_encoding
    /// https://en.bitcoin.it/wiki/Technical_background_of_version_1_Bitcoin_addresses
    /// </remarks>
    public static class Base58CheckEncoding
    {
        private const int ChecksumSize = 4;
        private const string Digits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        /// <summary>
        /// Encodes data with a 4-byte checksum
        /// </summary>
        /// <param name="data">Data to be encoded</param>
        /// <returns></returns>
        public static string Encode(byte[] data)
        {
            return EncodePlain(AddChecksum(data));
        }

        /// <summary>
        /// Encodes data in plain Base58, without any checksum.
        /// </summary>
        /// <param name="data">The data to be encoded</param>
        /// <returns></returns>
        public static string EncodePlain(byte[] data)
        {
            // Decode byte[] to BigInteger
            var intData = data.Aggregate<byte, BigInteger>(0, (current, t) => current * 256 + t);

            // Encode BigInteger to Base58 string
            var result = string.Empty;
            while (intData > 0)
            {
                var remainder = (int)(intData % 58);
                intData /= 58;
                result = Digits[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (var i = 0; i < data.Length && data[i] == 0; i++)
            {
                result = '1' + result;
            }

            return result;
        }

        /// <summary>
        /// Decodes data in Base58Check format (with 4 byte checksum)
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <returns>Returns decoded data if valid; throws FormatException if invalid</returns>
        public static byte[] Decode(string data)
        {
            var dataWithChecksum = DecodePlain(data);
            var dataWithoutChecksum = VerifyAndRemoveChecksum(dataWithChecksum);

            if (dataWithoutChecksum == null)
            {
                throw new FormatException("Base58 checksum is invalid");
            }

            return dataWithoutChecksum;
        }

        /// <summary>
        /// Decodes data in plain Base58, without any checksum.
        /// </summary>
        /// <param name="data">Data to be decoded</param>
        /// <returns>Returns decoded data if valid; throws FormatException if invalid</returns>
        public static byte[] DecodePlain(string data)
        {
            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (var i = 0; i < data.Length; i++)
            {
                var digit = Digits.IndexOf(data[i]); //Slow

                if (digit < 0)
                {
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", data[i], i));
                }

                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            var leadingZeroCount = data.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
              intData.ToByteArray()
              .Reverse()// to big endian
              .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();

            return result;
        }

        private static byte[] AddChecksum(byte[] data)
        {
            var checksum = GetChecksum(data);
            var dataWithChecksum = ArrayUtils.ConcatArrays(data, checksum);

            return dataWithChecksum;
        }

        //Returns null if the checksum is invalid
        private static byte[] VerifyAndRemoveChecksum(byte[] data)
        {
            var result = ArrayUtils.SubArray(data, 0, data.Length - ChecksumSize);
            var givenChecksum = ArrayUtils.SubArray(data, data.Length - ChecksumSize);
            var correctChecksum = GetChecksum(result);

            return givenChecksum.SequenceEqual(correctChecksum) ? result : null;
        }

        private static byte[] GetChecksum(byte[] data)
        {
            SHA256 sha256 = new SHA256Managed();
            var hash1 = sha256.ComputeHash(data);
            var hash2 = sha256.ComputeHash(hash1);

            var result = new byte[ChecksumSize];
            Array.Copy(hash2, 0, result, 0, result.Length);

            return result;
        }
    }
}