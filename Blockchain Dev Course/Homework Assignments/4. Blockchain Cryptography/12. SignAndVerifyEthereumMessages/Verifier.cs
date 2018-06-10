using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
using Newtonsoft.Json;
using System;
using System.IO;

namespace SignAndVerifyEthereumMessages
{
    internal static class Verifier
    {
        public static void Verify()
        {
            string input = File.ReadAllText("../../Output.json");
            EthereumSignature inputSignature = JsonConvert.DeserializeObject<EthereumSignature>(input);
            string message = inputSignature.Msg;
            byte[] messageBytes = Utils.GetBytes(message);
            byte[] messageHash = new Sha3Keccack().CalculateHash(messageBytes);

            byte[] rawSignature = inputSignature.Sig.HexToByteArray();
            byte[] r = ArrayUtils.SubArray(rawSignature, 0, 32);
            byte[] s = ArrayUtils.SubArray(rawSignature, 32, 32);
            int recId = rawSignature[rawSignature.Length - 1];

            EthECDSASignature signature = EthECDSASignatureFactory.FromComponents(r, s);

            EthECKey publicKey = EthECKey.RecoverFromSignature(signature, recId, messageHash);

            bool valid = publicKey.GetPublicAddress() == inputSignature.Address;
            Console.WriteLine("{0}", valid ? "Valid" : "Invalid");
        }
    }
}
