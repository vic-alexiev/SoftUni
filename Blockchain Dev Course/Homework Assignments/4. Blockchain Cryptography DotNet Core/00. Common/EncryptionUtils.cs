using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.KeyStore.Crypto;
using Nethereum.Signer;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
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
            byte[] key = Org.BouncyCastle.Crypto.Generators.SCrypt.Generate(
                passphrase,
                salt,
                cost,
                blockSize,
                parallelization,
                desiredKeyBitLength / 8);
            return key;
        }

        public static byte[] AesCtrEncrypt(byte[] message, byte[] iv, byte[] key)
        {
            KeyStoreCrypto keyStoreCrypto = new KeyStoreCrypto();
            byte[] encryptedMessage = keyStoreCrypto.GenerateAesCtrCipher(iv, key, message);
            return encryptedMessage;
        }

        public static byte[] AesCtrDecrypt(byte[] input, byte[] iv, byte[] key)
        {
            KeyStoreCrypto keyStoreCrypto = new KeyStoreCrypto();
            byte[] decryptedMessage = keyStoreCrypto.GenerateAesCtrDeCipher(iv, key, input);
            return decryptedMessage;
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

        public static byte[] RsaProcessMessage(
            bool forEncryption,
            byte[] message,
            AsymmetricKeyParameter key)
        {
            IAsymmetricBlockCipher cipher = new RsaBlindedEngine();
            cipher.Init(forEncryption, key);

            return cipher.ProcessBlock(message, 0, message.Length);
        }

        public static AsymmetricCipherKeyPair GenerateRsaKeyPair(
            BigInteger publicExponent,
            SecureRandom random,
            int strength,
            int certainty)
        {
            RsaKeyPairGenerator generator = new RsaKeyPairGenerator();
            RsaKeyGenerationParameters parameters = new RsaKeyGenerationParameters(
                publicExponent, random, strength, certainty);

            generator.Init(parameters);

            return generator.GenerateKeyPair();
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
