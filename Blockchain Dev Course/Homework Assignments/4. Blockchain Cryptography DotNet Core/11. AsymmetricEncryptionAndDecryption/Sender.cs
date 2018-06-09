using Common;
using Org.BouncyCastle.Crypto;
using System;
using System.IO;

namespace AsymmetricEncryptionAndDecryption
{
    internal static class Sender
    {
        public static void SendMessage(string message, AsymmetricKeyParameter rsaKey)
        {
            Console.WriteLine("Message before sending:\r\n" + message);
            byte[] messageBytes = Utils.GetBytes(message);

            // used as the key for AES encryption
            byte[] secret = EncryptionUtils.GetRandomBytes(32);
            Console.WriteLine("AES secret before sending:\r\n" + Utils.ToString(secret));

            // encrypt the shared secret using RSA
            byte[] encryptedSecret = EncryptionUtils.RsaProcessMessage(
                true, secret, rsaKey);

            // encrypt the message with the shared secret as key using AES
            byte[] iv = EncryptionUtils.GetRandomBytes(16);
            byte[] encryptedMessage = EncryptionUtils.AesCtrEncrypt(messageBytes, iv, secret);

            byte[] hmacKey = EncryptionUtils.GetRandomBytes(32);
            byte[] messageHmac = HashUtils.ComputeHmac(messageBytes, hmacKey, 256);

            TransferData transferData = new TransferData
            {
                AesEncryptedMessage = Utils.ToString(encryptedMessage),
                AesIV = Utils.ToString(iv),
                HmacKey = Utils.ToString(hmacKey),
                MessageHmac = Utils.ToString(messageHmac),
                RsaEncryptedSecret = Utils.ToString(encryptedSecret)
            };

            string output = JsonUtils.Serialize(transferData);
            File.WriteAllText("../../../TransferData.json", output);
        }
    }
}
