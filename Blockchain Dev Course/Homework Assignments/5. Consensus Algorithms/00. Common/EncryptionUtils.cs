﻿using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.KeyStore.Crypto;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Asn1.Sec;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Signers;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using Org.BouncyCastle.Security;
using System;

namespace Common
{
    public static class EncryptionUtils
    {
        private static readonly SecureRandom RandomGenerator = new SecureRandom();
        private static readonly X9ECParameters Secp256k1;
        private static readonly ECDomainParameters Secp256k1DomainParameters;

        static EncryptionUtils()
        {
            Secp256k1 = SecNamedCurves.GetByName("secp256k1");
            Secp256k1DomainParameters =
                new ECDomainParameters(Secp256k1.Curve, Secp256k1.G, Secp256k1.N, Secp256k1.H);
        }

        public static byte[] GetRandomBytes(int size)
        {

            byte[] data = new byte[size];
            RandomGenerator.NextBytes(data, 0, size);
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

        /// <summary>
        /// Generates an asymmetric key pair.
        /// </summary>
        /// <param name="strength">Key size</param>
        /// <returns></returns>
        public static AsymmetricCipherKeyPair GenerateAsymmetricKeyPair(int strength = 256)
        {
            ECKeyPairGenerator keyPairGenerator = new ECKeyPairGenerator();
            SecureRandom randomGenerator = new SecureRandom();
            KeyGenerationParameters parameters = new KeyGenerationParameters(
                randomGenerator, strength);

            keyPairGenerator.Init(parameters);

            return keyPairGenerator.GenerateKeyPair();
        }

        public static ECPoint GetPublicKey(BigInteger privateKey)
        {
            var q = Secp256k1.G.Multiply(privateKey);
            return q.Normalize();
        }

        public static ECPublicKeyParameters GetPublicKeyParameters(string privateKey)
        {
            BigInteger d = new BigInteger(privateKey, 16);
            var q = Secp256k1.G.Multiply(d);

            return new ECPublicKeyParameters(q, Secp256k1DomainParameters);
        }

        public static ECDSASignature Sign(byte[] data, BigInteger privateKey)
        {
            ECPrivateKeyParameters parameters =
                new ECPrivateKeyParameters(privateKey, Secp256k1DomainParameters);
            IDsaKCalculator kCalculator = new HMacDsaKCalculator(new Sha256Digest());
            IDsa signer = new ECDsaSigner(kCalculator);
            signer.Init(true, parameters);
            return new ECDSASignature(signer.GenerateSignature(data));
        }

        public static bool VerifySignature(
            byte[] hash,
            ECDSASignature signature,
            ECPublicKeyParameters parameters)
        {
            IDsaKCalculator kCalculator = new HMacDsaKCalculator(new Sha256Digest());
            var signer = new ECDsaSigner(kCalculator);
            signer.Init(false, parameters);
            return signer.VerifySignature(hash, signature.R, signature.S);
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
