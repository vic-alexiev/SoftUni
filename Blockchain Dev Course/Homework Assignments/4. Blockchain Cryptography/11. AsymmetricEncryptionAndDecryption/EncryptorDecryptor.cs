using Common;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;

namespace AsymmetricEncryptionAndDecryption
{
    internal class EncryptorDecryptor
    {
        private static void Main(string[] args)
        {
            AsymmetricCipherKeyPair rsaKeyPair = EncryptionUtils.GenerateRsaKeyPair(
                BigInteger.ValueOf(0x11), new SecureRandom(), 768, 25);

            Sender.SendMessage("exercise-cryptography", rsaKeyPair.Public);
            Receiver.ReceiveMessage(rsaKeyPair.Private);
        }
    }
}
