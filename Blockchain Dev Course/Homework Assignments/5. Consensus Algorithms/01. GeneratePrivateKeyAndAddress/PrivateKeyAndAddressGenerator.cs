using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;

namespace GeneratePrivateKeyAndAddress
{
    internal class PrivateKeyAndAddressGenerator
    {
        private static void Main(string[] args)
        {
            //string key = EncryptionUtils.GetRandomBytes(32).ToHex();
            //string key = "0e549dbcccfbd11e255f6037e1e640efaca0e19966ac77a592fdf06d295952a4";
            string key = "7e4670ae70c98d24f3662c172dc510a085578b9ccc717e6c2f4e547edd960a34";

            EthECKey privateKey = new EthECKey(key);
            byte[] privateKeyBytes = privateKey.GetPrivateKeyAsBytes();
            Console.WriteLine("Private key (64 hex digits):\r\n" + privateKeyBytes.ToHex());

            byte[] publicKeyBytes = privateKey.GetPubKeyNoPrefix();
            Console.WriteLine("Public key uncompressed (2 * 64 hex digits):\r\n" + publicKeyBytes.ToHex());

            ECPublicKeyParameters publicKeyParams = new ECKey(privateKeyBytes, true).GetPublicKeyParameters();
            string publicKeyCompressed = HashUtils.ToHexCompressed(publicKeyParams.Q);
            Console.WriteLine("Public key compressed (65 hex digits):\r\n" + publicKeyCompressed);

            byte[] ripemd160 = HashUtils.ComputeRIPEMD160(Utils.GetBytes(publicKeyCompressed));
            Console.WriteLine("Public address (40 hex digits):\r\n" + ripemd160.ToHex());
        }

        private static bool IsEven(BigInteger value)
        {
            return value.Remainder(BigInteger.Two) == BigInteger.Zero;
        }
    }
}
