using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto;
using System;
using System.IO;

namespace AsymmetricEncryptionAndDecryption
{
    internal class Receiver
    {
        public static void ReceiveMessage(AsymmetricKeyParameter rsaKey)
        {
            string transfer = File.ReadAllText("../../../TransferData.json");
            TransferData transferData = JsonConvert.DeserializeObject<TransferData>(transfer);

            byte[] encryptedSecret = transferData.RsaEncryptedSecret.HexToByteArray();
            byte[] decryptedSecret = EncryptionUtils.RsaProcessMessage(
                false, encryptedSecret, rsaKey);
            Console.WriteLine("Transfer data received. Decrypted secret (RSA):\r\n" + Utils.ToString(decryptedSecret));

            byte[] encryptedMessage = transferData.AesEncryptedMessage.HexToByteArray();
            byte[] iv = transferData.AesIV.HexToByteArray();
            byte[] decryptedMessage = EncryptionUtils.AesCtrDecrypt(
                encryptedMessage, iv, decryptedSecret);
            Console.WriteLine("Decrytped message (AES):\r\n" + Utils.ToUTF8String(decryptedMessage));

            byte[] hmacKey = transferData.HmacKey.HexToByteArray();
            byte[] messageHmac = HashUtils.ComputeHmac(decryptedMessage, hmacKey, 256);
            string decryptedMessageHmac = Utils.ToString(messageHmac);
            bool validMessage = decryptedMessageHmac == transferData.MessageHmac;
            Console.WriteLine(
                "Decrytped message integrity check:\r\n" +
                (validMessage ? "Passed" : "Failed"));
        }
    }
}
