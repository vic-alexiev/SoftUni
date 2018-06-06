using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography;

namespace Common
{
    public static class EncryptionUtils
    {
        private static RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

        public static byte[] GetRandomBytes(int size)
        {
            byte[] data = new byte[size];
            _rng.GetBytes(data);
            return data;
        }

        public static byte[] GenerateSCryptKey(
            byte[] passphrase,
            byte[] salt,
            int cost,
            int blockSize,
            int parallelization,
            int desiredKeyBitLength)
        {
            byte[] key = SCrypt.Generate(
                passphrase,
                salt,
                cost,
                blockSize,
                parallelization,
                desiredKeyBitLength / 8);
            return key;
        }

        public static BufferedBlockCipher GetTwofishCipher()
        {
            BufferedBlockCipher cipher = new PaddedBufferedBlockCipher(
                new CbcBlockCipher(new TwofishEngine()));
            return cipher;
        }

        public static string BlockCipherEncrypt(
            IBufferedCipher cipher,
            byte[] message,
            byte[] key,
            byte[] iv = null)
        {
            cipher.Init(true, new ParametersWithIV(
                new KeyParameter(key),
                iv ?? GetRandomBytes(cipher.GetBlockSize())));
            byte[] result = cipher.DoFinal(message);
            return Utils.ToString(result);
        }
    }
}
