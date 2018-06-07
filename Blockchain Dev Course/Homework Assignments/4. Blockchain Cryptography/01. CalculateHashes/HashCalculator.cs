using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using System;

namespace CalculateHashes
{
    class HashCalculator
    {
        private static void Main(string[] args)
        {
            string message = "blockchain";
            byte[] messageData = Utils.GetBytes(message);
            Console.WriteLine("SHA-384:{0}{1}", Environment.NewLine, HashUtils.ComputeSha384(messageData).ToHex());
            Console.WriteLine("SHA-512:{0}{1}", Environment.NewLine, HashUtils.ComputeSha512(messageData).ToHex());
            Console.WriteLine("SHA3-512:{0}{1}", Environment.NewLine, HashUtils.ComputeSha3Digest(messageData, 512).ToHex());
            Console.WriteLine("Keccak-512:{0}{1}", Environment.NewLine, HashUtils.ComputeSha3KeccakDigest(messageData, 512).ToHex());
            Console.WriteLine("Whirlpool(512):{0}{1}", Environment.NewLine, HashUtils.ComputeWhirlpoolDigest(messageData).ToHex());
        }
    }
}
