using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using System.Security.Cryptography;

namespace Common
{
    public static class HashUtils
    {
        public static string ComputeSha384(byte[] data)
        {
            SHA384 sha384 = new SHA384Managed();
            byte[] result = sha384.ComputeHash(data);
            return Utils.ToString(result);
        }

        public static string ComputeSha512(byte[] data)
        {
            SHA512 sha512 = new SHA512Managed();
            byte[] result = sha512.ComputeHash(data);
            return Utils.ToString(result);
        }

        public static string ComputeSha3Digest(byte[] data, int bitLength)
        {
            var digest = new Sha3Digest(bitLength);
            byte[] buffer = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(buffer, 0);
            return Hex.ToHexString(buffer);
        }

        public static string ComputeSha3KeccakDigest(byte[] data, int bitLength)
        {
            var digest = new KeccakDigest(bitLength);
            byte[] buffer = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(buffer, 0);
            return Hex.ToHexString(buffer);
        }

        public static string ComputeWhirlpoolDigest(byte[] data)
        {
            IDigest digest = new WhirlpoolDigest();
            byte[] buffer = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(buffer, 0);
            return Hex.ToHexString(buffer);
        }

        public static string ComputeHmac(string algorithmName, byte[] message, byte[] key)
        {
            HMAC hmac = HMAC.Create(algorithmName);
            hmac.Key = key;
            byte[] data = hmac.ComputeHash(message);
            return Utils.ToString(data);
        }
    }
}
