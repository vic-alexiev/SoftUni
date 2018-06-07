using Nethereum.Hex.HexConvertors.Extensions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Security.Cryptography;

namespace Common
{
    public static class HashUtils
    {
        public static string CreateBitcoinAddress(string hexHash)
        {
            byte[] step1 = hexHash.HexToByteArray();
            byte[] step2_SHA256 = ComputeSha256(step1);
            byte[] step3_RIPEMD160 = ComputeRIPEMD160(step2_SHA256);
            byte[] step4_prepend_byte = ArrayUtils.ConcatArrays(new byte[1], step3_RIPEMD160);
            return Base58CheckEncoding.Encode(step4_prepend_byte);
        }

        public static byte[] ComputeRIPEMD160(byte[] data)
        {
            RIPEMD160Managed ripemd160 = new RIPEMD160Managed();
            return ripemd160.ComputeHash(data);
        }

        public static byte[] ComputeSha256(byte[] data)
        {
            SHA256Managed sha256 = new SHA256Managed();
            return sha256.ComputeHash(data);
        }

        public static byte[] ComputeSha384(byte[] data)
        {
            SHA384 sha384 = new SHA384Managed();
            return sha384.ComputeHash(data);
        }

        public static byte[] ComputeSha512(byte[] data)
        {
            SHA512 sha512 = new SHA512Managed();
            return sha512.ComputeHash(data);
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
            HMAC hmac = HMAC.Create(algorithmName);
            hmac.Key = key;
            return hmac.ComputeHash(message);
        }
    }
}
