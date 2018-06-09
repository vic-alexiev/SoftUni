using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
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
            byte[] messageBytes = Utils.GetBytes(inputSignature.Message);
            byte[] messageHash = new Sha3Keccack().CalculateHash(messageBytes);
            int recId = inputSignature.V.HexToByteArray()[0];

            EthECDSASignature signature = EthECDSASignatureFactory.FromComponents(
                inputSignature.R.HexToByteArray(),
                inputSignature.S.HexToByteArray());

            EthECKey publicKey = EthECKey.RecoverFromSignature(signature, recId, messageHash);
            Console.WriteLine("Public address: {0}", publicKey.GetPublicAddress());
        }
    }
}
