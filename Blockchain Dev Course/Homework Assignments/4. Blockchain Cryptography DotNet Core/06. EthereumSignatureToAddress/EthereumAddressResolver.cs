using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Newtonsoft.Json;
using System;
using System.IO;

namespace EthereumSignatureToAddress
{
    internal class EthereumAddressResolver
    {
        private static void Main(string[] args)
        {
            string input = File.ReadAllText("../../../Input.json");
            EthereumSignature inputSignature = JsonConvert.DeserializeObject<EthereumSignature>(input);
            byte[] messageHash = inputSignature.Hash.HexToByteArray();
            int recId = inputSignature.V.HexToByteArray()[0];

            EthECDSASignature signature = EthECDSASignatureFactory.FromComponents(
                inputSignature.R.HexToByteArray(),
                inputSignature.S.HexToByteArray());

            EthECKey publicKey = EthECKey.RecoverFromSignature(signature, recId, messageHash);
            Console.WriteLine("Public address: {0}", publicKey.GetPublicAddress());
        }
    }
}
