using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using System;

namespace BitcoinAddressGenerator
{
    internal class BitcoinAddressGenerator
    {
        private static void Main(string[] args)
        {
            string hexHash = "0450863AD64A87AE8A2FE83C1AF1A8403CB53F53E486D8511DAD8A04887E5B23522CD470243453A299FA9E77237716103ABC11A1DF38855ED6F2EE187E9C582BA6";
            string output = HashUtils.CreateBitcoinAddressVerbose(hexHash.HexToByteArray());
            Console.WriteLine(output);

            string check = HashUtils.PublicKeyToBitcoinAddress(hexHash);
            Console.WriteLine("Check:\r\n" + check);
        }
    }
}
