using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using System;
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

        public static byte[] BlockCipherProcessMessage(
            IBufferedCipher cipher,
            bool forEncryption,
            byte[] message,
            byte[] key,
            byte[] iv = null)
        {
            cipher.Init(forEncryption, new ParametersWithIV(
                new KeyParameter(key),
                iv ?? GetRandomBytes(cipher.GetBlockSize())));
            byte[] result = cipher.DoFinal(message);
            return result;
        }

        public static string ToString(EthECDSASignature signature)
        {
            byte[] result = new byte[65];
            byte[] rands = signature.To64ByteArray();
            Array.Copy(rands, result, rands.Length);
            result[result.Length - 1] = (byte)EthECKey.GetRecIdFromV(signature.V);
            return result.ToHex(true);
        }
    }
}
