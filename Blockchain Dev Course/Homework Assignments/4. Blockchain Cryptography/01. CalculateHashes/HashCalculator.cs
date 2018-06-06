using Common;
using System;

namespace CalculateHashes
{
    class HashCalculator
    {
        private static void Main(string[] args)
        {
            string message = "blockchain";
            byte[] messageData = Utils.GetBytes(message);
            Console.WriteLine("SHA-384:{0}{1}", Environment.NewLine, HashUtils.ComputeSha384(messageData));
            Console.WriteLine("SHA-512:{0}{1}", Environment.NewLine, HashUtils.ComputeSha512(messageData));
            Console.WriteLine("SHA3-512:{0}{1}", Environment.NewLine, HashUtils.ComputeSha3Digest(messageData, 512));
            Console.WriteLine("Keccak-512:{0}{1}", Environment.NewLine, HashUtils.ComputeSha3KeccakDigest(messageData, 512));
            Console.WriteLine("Whirlpool(512):{0}{1}", Environment.NewLine, HashUtils.ComputeWhirlpoolDigest(messageData));
        }
    }
}
