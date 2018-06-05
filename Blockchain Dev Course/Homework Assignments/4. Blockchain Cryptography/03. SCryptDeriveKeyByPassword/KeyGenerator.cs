using Common;
using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Security.Cryptography;

namespace SCryptDeriveKeyByPassword
{
    internal class KeyGenerator
    {
        static void Main(string[] args)
        {
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                string passphrase = "p@$$w0rd~3";
                int saltBitLength = 256;
                byte[] salt = new byte[saltBitLength / 8];
                rng.GetBytes(salt);

                Console.WriteLine("scrypt key:{0}{1}",
                    Environment.NewLine,
                    GenerateKey(passphrase, salt, 16384, 16, 1, 256));
            }
        }

        private static string GenerateKey(
            string passphrase,
            byte[] salt,
            int cost,
            int blockSize,
            int parallelization,
            int desiredKeyBitLength)
        {
            byte[] data = SCrypt.Generate(
                Utils.GetBytes(passphrase),
                salt,
                cost,
                blockSize,
                parallelization,
                desiredKeyBitLength / 8);
            return Utils.ToString(data);
        }
    }
}
