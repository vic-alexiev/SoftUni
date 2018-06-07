using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
using System;

namespace EthereumSignatureVerifier
{
    internal class EthereumSignatureVerifier
    {
        private static void Main(string[] args)
        {
            //EthECKey privateKey = EthECKey.GenerateKey();
            EthECKey privateKey = new EthECKey("97ddae0f3a25b92268175400149d65d6887b9cefaf28ea2c078e05cdc15a3c0a");
            string publicAddress = privateKey.GetPublicAddress();
            string message = "exercise-cryptography";
            byte[] messageBytes = Utils.GetBytes(message);
            byte[] messageHash = new Sha3Keccack().CalculateHash(messageBytes);

            //EthECDSASignature validSignature = privateKey.SignAndCalculateV(messageHash);
            EthECDSASignature validSignature = EthECDSASignatureFactory.FromComponents(
                "acd0acd4eabd1bec05393b33b4018fa38b69eba8f16ac3d60eec9f4d2abc127e".HexToByteArray(),
                "3c92939e680b91b094242af80fce6f217a34197a69d35edaf616cb0c3da4265b".HexToByteArray(),
                28);

            //EthECKey publicKey = new EthECKey(privateKey.GetPubKey(), false);
            EthECKey publicKey = EthECKey.RecoverFromSignature(validSignature, messageHash);

            VerifySignatureAndPrintResult(messageHash, publicKey, validSignature);

            EthECDSASignature invalidSignature = EthECDSASignatureFactory.FromComponents(
                "5550acd4eabd1bec05393b33b4018fa38b69eba8f16ac3d60eec9f4d2abc127e".HexToByteArray(),
                "3c92939e680b91b094242af80fce6f217a34197a69d35edaf616cb0c3da4265b".HexToByteArray(),
                28);

            VerifySignatureAndPrintResult(messageHash, publicKey, invalidSignature);
        }

        private static void VerifySignatureAndPrintResult(byte[] messageHash, EthECKey publicKey, EthECDSASignature signature)
        {
            bool valid = publicKey.Verify(messageHash, signature);
            Console.WriteLine("{0}", valid ? "valid" : "invalid");
        }
    }
}
