using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer.Crypto;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Math.EC;
using System;

namespace SignAndVerifyTransaction
{
    internal class SignerAndVerifier
    {
        private static void Main(string[] args)
        {
            //GenerateAddressFromPrivateKey();
            SignAndVerifyTransaction(
                "7e4670ae70c98d24f3662c172dc510a085578b9ccc717e6c2f4e547edd960a34",
                "f51362b7351ef62253a227a77751ad9b2302f911",
                "2018-02-10T17:53:48.972Z",
                25000,
                10);
        }

        private static void GenerateAddressFromPrivateKey()
        {
            Console.WriteLine("Random private key --> public key --> address");
            Console.WriteLine("---------------------------------------------");

            AsymmetricCipherKeyPair keyPair = EncryptionUtils.GenerateAsymmetricKeyPair();

            BigInteger privateKey = ((ECPrivateKeyParameters)keyPair.Private).D;
            Console.WriteLine("Private key (64 hex digits):\r\n" + privateKey.ToString(16));
            Console.WriteLine("Private key (BigInteger):\r\n" + privateKey.ToString(10));

            ECPoint publicKey = ((ECPublicKeyParameters)keyPair.Public).Q;
            Console.WriteLine("Public key (2 * BigIntegers):\r\n({0}, {1})",
                publicKey.XCoord.ToBigInteger(),
                publicKey.YCoord.ToBigInteger());
            Console.WriteLine("Public key (2 * 64 hex digits):\r\n({0}, {1})",
                publicKey.XCoord,
                publicKey.YCoord);

            string publicKeyCompressed = HashUtils.ToHexCompressed(publicKey);
            Console.WriteLine("Public key compressed (65 hex digits):\r\n" +
                publicKeyCompressed);

            byte[] publicAddress = HashUtils.ComputeRIPEMD160(Utils.GetBytes(publicKeyCompressed));
            Console.WriteLine("Public address (40 hex digits):\r\n" + publicAddress.ToHex());
        }

        private static void SignAndVerifyTransaction(
            string privateKey,
            string recipientAddress,
            string dateCreatedIso8601,
            int value,
            int fee)
        {
            Console.WriteLine("Generate and sign a transaction");
            Console.WriteLine("-------------------------------");

            Console.WriteLine("Sender private key:\r\n" + privateKey);

            BigInteger senderPrivateKey = new BigInteger(privateKey, 16);
            ECPoint senderPublicKey = EncryptionUtils.GetPublicKey(senderPrivateKey);
            string senderPublicKeyCompressed = HashUtils.ToHexCompressed(senderPublicKey);
            Console.WriteLine("Sender public key compressed (65 hex digits):\r\n" +
                senderPublicKeyCompressed);

            string senderAddress = HashUtils.ComputeRIPEMD160(
                Utils.GetBytes(senderPublicKeyCompressed)).ToHex();
            Console.WriteLine("Sender address (40 hex digits):\r\n" + senderAddress);

            Transaction transaction = new Transaction
            {
                Sender = senderAddress,
                Recipient = recipientAddress,
                SenderPublicKey = senderPublicKeyCompressed,
                Value = value,
                Fee = fee,
                DateCreatedIso8601 = dateCreatedIso8601
            };

            string transactionJsonFormatted = JsonUtils.Serialize(transaction);
            Console.WriteLine("Transaction (JSON, formatted):\r\n" + transactionJsonFormatted);

            string transactionJson = JsonUtils.Serialize(transaction, false);
            Console.WriteLine("Transaction (JSON):\r\n" + transactionJson);

            byte[] transactionHash = HashUtils.ComputeSha256(Utils.GetBytes(transactionJson));
            Console.WriteLine("Transaction hash (SHA256):\r\n" + transactionHash.ToHex());

            ECDSASignature transactionSignature =
                EncryptionUtils.Sign(transactionHash, senderPrivateKey);
            string r = transactionSignature.R.ToString(16);
            string s = transactionSignature.S.ToString(16);

            Console.WriteLine("Transaction signature:\r\n({0}, {1})", r, s);

            transaction.Signature = new Signature
            {
                R = r,
                S = s
            };

            string signedTransactionJson = JsonUtils.Serialize(transaction);
            Console.WriteLine("Signed transaction (JSON):\r\n" + signedTransactionJson);

            ECPublicKeyParameters parameters =
                EncryptionUtils.GetPublicKeyParameters(privateKey);
            bool valid = EncryptionUtils.VerifySignature(
                transactionHash, transactionSignature, parameters);
            Console.WriteLine("Signature valid: " + valid);
        }
    }
}
