using Nethereum.Signer;
using System;

namespace EthereumSignatureToAddress
{
    internal class Program
    {
        private const int EthereumAddressLength = 20;

        private static void Main(string[] args)
        {
            //EthECKey privateKey = EthECKey.GenerateKey();
            EthECKey privateKey = new EthECKey("97ddae0f3a25b92268175400149d65d6887b9cefaf28ea2c078e05cdc15a3c0a");
            Console.WriteLine("Public address: {0}", privateKey.GetPublicAddress());
        }
    }
}
