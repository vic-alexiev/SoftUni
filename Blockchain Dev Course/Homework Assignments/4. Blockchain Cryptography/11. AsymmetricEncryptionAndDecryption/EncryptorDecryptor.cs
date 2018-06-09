using Common;
using NBitcoin;
using Nethereum.KeyStore.Crypto;
using Nethereum.Signer;
using System;

namespace AsymmetricEncryptionAndDecryption
{
    internal class EncryptorDecryptor
    {
        private static void Main(string[] args)
        {
            string message = "exercise-cryptography";

            // 1. using Nethereum
            AesEncryptDecrypt(message, EthECKey.GenerateKey().GetPubKeyNoPrefix());

            // 2. Using NBitcoin
            Key privateKey = new Key(EncryptionUtils.GetRandomBytes(32));
            PubKey secret = privateKey.PubKey.GetSharedPubkey(privateKey);
            AesEncryptDecrypt(message, secret.ToBytes());
        }

        private static void AesEncryptDecrypt(string message, byte[] secret)
        {
            byte[] iv = EncryptionUtils.GetRandomBytes(16);
            byte[] sharedSecret = HashUtils.ComputeHmac("HMACSHA256", secret, EncryptionUtils.GetRandomBytes(32));

            KeyStoreCrypto keyStoreCrypto = new KeyStoreCrypto();
            byte[] encryptedMessage = keyStoreCrypto.GenerateAesCtrCipher(iv, sharedSecret, Common.Utils.GetBytes(message));

            byte[] decryptedMessage = keyStoreCrypto.GenerateAesCtrDeCipher(iv, sharedSecret, encryptedMessage);
            Console.WriteLine("Decrytped message: " + Common.Utils.ToUTF8String(decryptedMessage));
        }
    }
}
