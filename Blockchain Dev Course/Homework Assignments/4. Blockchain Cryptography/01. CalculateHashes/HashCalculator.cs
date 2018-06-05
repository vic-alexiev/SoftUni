using Common;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Security.Cryptography;

namespace CalculateHashes
{
    class HashCalculator
    {
        private static void Main(string[] args)
        {
            string message = "blockchain";
            Console.WriteLine("SHA-384:{0}{1}", Environment.NewLine, CalcSha384(message));
            Console.WriteLine("SHA-512:{0}{1}", Environment.NewLine, CalcSha512(message));
            Console.WriteLine("SHA3-512:{0}{1}", Environment.NewLine, CalcSha3Digest(message, 512));
            Console.WriteLine("Keccak-512:{0}{1}", Environment.NewLine, CalcSha3KeccakDigest(message, 512));
            Console.WriteLine("Whirlpool(512):{0}{1}", Environment.NewLine, CalcWhirlpoolDigest(message));
        }

        private static string CalcSha384(string value)
        {
            byte[] data = Utils.GetBytes(value);
            SHA384 shaM = new SHA384Managed();
            byte[] result = shaM.ComputeHash(data);
            return Utils.ToString(result);
        }

        private static string CalcSha512(string value)
        {
            byte[] data = Utils.GetBytes(value);
            SHA512 shaM = new SHA512Managed();
            byte[] result = shaM.ComputeHash(data);
            return Utils.ToString(result);
        }

        private static string CalcSha3Digest(string value, int bitLength)
        {
            byte[] data = Utils.GetBytes(value);
            var digest = new Sha3Digest(bitLength);
            byte[] buffer = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(buffer, 0);
            return Hex.ToHexString(buffer);
        }

        private static string CalcSha3KeccakDigest(string value, int bitLength)
        {
            byte[] data = Utils.GetBytes(value);
            var digest = new KeccakDigest(bitLength);
            byte[] buffer = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(buffer, 0);
            return Hex.ToHexString(buffer);
        }

        private static string CalcWhirlpoolDigest(string value)
        {
            byte[] data = Utils.GetBytes(value);
            IDigest digest = new WhirlpoolDigest();
            byte[] buffer = new byte[digest.GetDigestSize()];
            digest.BlockUpdate(data, 0, data.Length);
            digest.DoFinal(buffer, 0);
            return Hex.ToHexString(buffer);
        }
    }
}
