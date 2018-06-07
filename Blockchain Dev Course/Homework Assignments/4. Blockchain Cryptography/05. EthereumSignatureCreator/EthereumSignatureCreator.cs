using System;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Util;
using Common;

namespace EthereumSignatureCreator
{
    internal class EthereumSignatureCreator
    {
        private static void Main(string[] args)
        {
            EthECKey privateKey = EthECKey.GenerateKey();
            //EthECKey privateKey = new EthECKey("97ddae0f3a25b92268175400149d65d6887b9cefaf28ea2c078e05cdc15a3c0a");
            string message = "exercise-cryptography";
            byte[] messageBytes = Utils.GetBytes(message);
            byte[] messageHash = new Sha3Keccack().CalculateHash(messageBytes);

            EthECDSASignature signature = privateKey.SignAndCalculateV(messageHash);

            EthereumSignature result = new EthereumSignature
            {
                Signature = ToString(signature),
                V = string.Format("0x{0:X2}", EthECKey.GetRecIdFromV(signature.V)),
                R = signature.R.ToHex(true),
                S = signature.S.ToHex(true)
            };

            Console.WriteLine(JsonUtils.Serialize(result));
        }

        private static string ToString(EthECDSASignature signature)
        {
            byte[] result = new byte[65];
            byte[] rands = signature.To64ByteArray();
            Array.Copy(rands, result, rands.Length);
            result[result.Length - 1] = (byte)EthECKey.GetRecIdFromV(signature.V);
            return result.ToHex(true);
        }
    }
}
