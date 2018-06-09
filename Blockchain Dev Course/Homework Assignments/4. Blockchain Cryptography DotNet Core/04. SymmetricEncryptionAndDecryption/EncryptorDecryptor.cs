using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using Org.BouncyCastle.Crypto;
using System;
using System.IO;

namespace SymmetricEncryptionAndDecryption
{
    internal class EncryptorDecryptor
    {
        private static void Main(string[] args)
        {
            byte[] passphrase = Utils.GetBytes("p@$$w0rd~3");
            byte[] message = Utils.GetBytes("exercise-cryptography");

            //byte[] scryptSalt = EncryptionUtils.GetRandomBytes(256 / 8);
            byte[] scryptSalt = "7b07a2977a473e84fc30d463a2333bcfea6cb3400b16bec4e17fe981c925ba4f".HexToByteArray();
            int scryptCost = 16384;
            int scryptBlockSize = 16;
            int scryptParallelization = 1;
            int scryptDesiredKeyBitLength = 512;

            byte[] scryptKey = EncryptionUtils.GenerateSCryptKey(
                passphrase,
                scryptSalt,
                scryptCost,
                scryptBlockSize,
                scryptParallelization,
                scryptDesiredKeyBitLength);

            Console.WriteLine("Scrypt key: " + Utils.ToString(scryptKey));

            byte[] encryptionKey = new byte[scryptKey.Length / 2];
            Array.Copy(scryptKey, encryptionKey, encryptionKey.Length);
            Console.WriteLine("Encryption key: " + Utils.ToString(encryptionKey));

            byte[] hmacKey = new byte[scryptKey.Length / 2];
            Array.Copy(scryptKey, hmacKey.Length, hmacKey, 0, hmacKey.Length);
            Console.WriteLine("HMAC-SHA256 key: " + Utils.ToString(hmacKey));

            BufferedBlockCipher twofishCipher = EncryptionUtils.GetTwofishCipher();
            int cipherBlockSize = twofishCipher.GetBlockSize();
            //byte[] iv = EncryptionUtils.GetRandomBytes(cipherBlockSize);
            byte[] iv = "433e0d8557a800a40c1d3b54f6636ff5".HexToByteArray();

            byte[] twofishEncryptedMessage = EncryptionUtils.BlockCipherProcessMessage(
                twofishCipher,
                true,
                message,
                encryptionKey,
                iv);

            string hmacHashedMessage =
                HashUtils.ComputeHmac(message, hmacKey, 256).ToHex();

            string outputJson = GetEncryptionResultJson(
                scryptSalt,
                scryptCost,
                scryptBlockSize,
                scryptParallelization,
                scryptDesiredKeyBitLength,
                iv,
                twofishEncryptedMessage,
                hmacHashedMessage);

            File.WriteAllText("../../../Output.json", outputJson);
            Console.WriteLine(outputJson);

            byte[] twofishDecryptedMessage = EncryptionUtils.BlockCipherProcessMessage(
                twofishCipher,
                false,
                twofishEncryptedMessage,
                encryptionKey,
                iv);

            Console.WriteLine("Decrypted message: {0}", Utils.ToUTF8String(twofishDecryptedMessage));
        }

        private static string GetEncryptionResultJson(
            byte[] scryptSalt,
            int scryptCost,
            int scryptBlockSize,
            int scryptParallelization,
            int scryptDesiredKeyBitLength,
            byte[] iv,
            byte[] twofishEncryptedMessage,
            string hmacHashedMessage)
        {
            EncryptionResult result = new EncryptionResult
            {
                Scrypt = new SCryptParams
                {
                    Dklen = scryptDesiredKeyBitLength / 8,
                    Salt = Utils.ToString(scryptSalt),
                    N = scryptCost,
                    R = scryptBlockSize,
                    P = scryptParallelization
                },
                Twofish = Utils.ToString(twofishEncryptedMessage),
                IV = Utils.ToString(iv),
                Mac = hmacHashedMessage
            };

            return JsonUtils.Serialize(result);
        }
    }
}
