using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
using Newtonsoft.Json;
using System;
using System.IO;

namespace EthereumSignatureVerifier
{
    internal class EthereumSignatureVerifier
    {
        private static void Main(string[] args)
        {
            string input = File.ReadAllText("../../Input.json");
            EthereumSignature inputSignature = JsonConvert.DeserializeObject<EthereumSignature>(input);
            string message = inputSignature.Message;
            byte[] messageBytes = Utils.GetBytes(message);
            byte[] messageHash = new Sha3Keccack().CalculateHash(messageBytes);
            int recId = inputSignature.V.HexToByteArray()[0];

            EthECDSASignature signature = EthECDSASignatureFactory.FromComponents(
                inputSignature.R.HexToByteArray(),
                inputSignature.S.HexToByteArray());

            EthECKey publicKey = EthECKey.RecoverFromSignature(signature, recId, messageHash);

            VerifySignatureAndPrintResult(publicKey, inputSignature);
        }

        private static void VerifySignatureAndPrintResult(EthECKey publicKey, EthereumSignature signature)
        {
            bool valid = publicKey.GetPublicAddress() == signature.Address;
            Console.WriteLine("{0}", valid ? "valid" : "invalid");
        }
    }
}
