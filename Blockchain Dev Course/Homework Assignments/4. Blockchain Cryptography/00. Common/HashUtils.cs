﻿using NBitcoin;
using NBitcoin.DataEncoders;
using Nethereum.Hex.HexConvertors.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    public static class HashUtils
    {
        /// <summary>
        /// Creates a Bitcoin address step by step.
        /// </summary>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        /// <remarks>
        /// See here for more details:
        /// https://en.bitcoin.it/wiki/Technical_background_of_version_1_Bitcoin_addresses
        /// </remarks>
        public static string CreateBitcoinAddressVerbose(byte[] publicKey)
        {
            StringBuilder output = new StringBuilder();
            byte[] step1 = publicKey;
            output.AppendLine("1. public_key:\r\n" + step1.ToHex());

            byte[] step2_SHA256 = ComputeSha256(step1);
            output.AppendLine("2. SHA256(public_key):\r\n" + step2_SHA256.ToHex());

            byte[] step3_RIPEMD160 = ComputeRIPEMD160(step2_SHA256);
            output.AppendLine("3. RIPEMD160(SHA256(public_key)):\r\n" + step3_RIPEMD160.ToHex());

            byte[] step4_prepend_byte = ArrayUtils.ConcatArrays(new byte[1], step3_RIPEMD160);
            output.AppendLine("4. [0x00] + RIPEMD160(SHA256(public_key)):\r\n" + step4_prepend_byte.ToHex());

            byte[] step5_SHA256 = ComputeSha256(step4_prepend_byte);
            output.AppendLine("5. SHA256([0x00] + RIPEMD160(SHA256(public_key))):\r\n" + step5_SHA256.ToHex());

            byte[] step6_SHA256 = ComputeSha256(step5_SHA256);
            output.AppendLine("6. SHA256(SHA256([0x00] + RIPEMD160(SHA256(public_key)))):\r\n" + step6_SHA256.ToHex());

            byte[] step7_checksum = ArrayUtils.SubArray(step6_SHA256, 0, 4);
            output.AppendLine("7. checksum = First4Bytes(SHA256(SHA256([0x00] + RIPEMD160(SHA256(public_key))))):\r\n" + step7_checksum.ToHex());

            byte[] step8_BTC_address = ArrayUtils.ConcatArrays(step4_prepend_byte, step7_checksum);
            output.AppendLine("8. BTC_address = [0x00] + RIPEMD160(SHA256(public_key)) + checksum:\r\n" + step8_BTC_address.ToHex());

            string step9_Base58Check_encode = new Base58Encoder().EncodeData(step8_BTC_address);
            output.AppendLine("9. Base58CheckEncode(BTC_address):\r\n" + step9_Base58Check_encode);

            return output.ToString();
        }

        public static string WifToBitcoinAddress(string wif, Network network = null)
        {
            Key privateKey = Key.Parse(wif);
            return CreateBitcoinAddress(privateKey.PubKey, network);
        }

        public static string PublicKeyToBitcoinAddress(string hex, Network network = null)
        {
            PubKey publicKey = new PubKey(hex);
            return CreateBitcoinAddress(publicKey, network);
        }

        public static byte[] ComputeRIPEMD160(byte[] data)
        {
            using (RIPEMD160Managed ripemd160 = new RIPEMD160Managed())
            {
                return ripemd160.ComputeHash(data);
            }
        }

        public static byte[] ComputeSha256(byte[] data)
        {
            using (SHA256Managed sha256 = new SHA256Managed())
            {
                return sha256.ComputeHash(data);
            }
        }

        public static byte[] ComputeSha384(byte[] data)
        {
            using (SHA384 sha384 = new SHA384Managed())
            {
                return sha384.ComputeHash(data);
            }
        }

        public static byte[] ComputeSha512(byte[] data)
        {
            using (SHA512 sha512 = new SHA512Managed())
            {
                return sha512.ComputeHash(data);
            }
        }

        public static byte[] ComputeSha3Digest(byte[] data, int bitLength)
        {
            var digest = new Sha3Digest(bitLength);
            byte[] buffer = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(buffer, 0);
            return buffer;
        }

        public static byte[] ComputeSha3KeccakDigest(byte[] data, int bitLength)
        {
            var digest = new KeccakDigest(bitLength);
            byte[] buffer = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(buffer, 0);
            return buffer;
        }

        public static byte[] ComputeWhirlpoolDigest(byte[] data)
        {
            IDigest digest = new WhirlpoolDigest();
            byte[] buffer = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(buffer, 0);
            return buffer;
        }

        public static byte[] ComputeHmac(string algorithmName, byte[] message, byte[] key)
        {
            using (HMAC hmac = HMAC.Create(algorithmName))
            {
                hmac.Key = key;
                return hmac.ComputeHash(message);
            }
        }

        private static string CreateBitcoinAddress(PubKey publicKey, Network network = null)
        {
            return publicKey.GetAddress(network ?? Network.Main).ToString();
        }
    }
}
