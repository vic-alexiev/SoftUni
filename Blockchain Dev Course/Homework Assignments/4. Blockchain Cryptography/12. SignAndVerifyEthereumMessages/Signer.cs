using Common;
using Nethereum.Signer;
using Nethereum.Util;
using System;
using System.IO;

namespace SignAndVerifyEthereumMessages
{
    internal static class Signer
    {
        public static void Sign()
        {
            string[] input = File.ReadAllLines("../../Input.txt");
            string inputKey = input[0];
            string inputMessage = input[1];

            EthECKey privateKey = new EthECKey(inputKey);
            byte[] messageBytes = Utils.GetBytes(inputMessage);
            byte[] messageHash = new Sha3Keccack().CalculateHash(messageBytes);

            EthECDSASignature ethECDSASignature = privateKey.SignAndCalculateV(messageHash);
            EthereumSignature signature = new EthereumSignature
            {
                Address = privateKey.GetPublicAddress(),
                Msg = inputMessage,
                Sig = EncryptionUtils.ToString(ethECDSASignature),
                Version = "1"
            };
            string output = JsonUtils.Serialize(signature);
            File.WriteAllText("../../Output.json", output);
            Console.WriteLine(output);
        }
    }
}
