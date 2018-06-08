using Common;
using NBitcoin;
using NBitcoin.DataEncoders;
using Nethereum.Hex.HexConvertors.Extensions;
using System;

namespace PrivateKeyToBitcoinAddress
{
    class BitcoinAddressGenerator
    {
        static void Main(string[] args)
        {
            string wif = "KxyJg5ZCk6SFsorqiR7Bb6heDDmss4PX76VtyM5qTEyn83Bgt1tD";
            Key privateKey = Key.Parse(wif);
            Console.WriteLine("0. private_key:\r\n" + privateKey.ToBytes().ToHex());

            PubKey publicKey = privateKey.PubKey;
            Console.WriteLine("00. public_key_uncompressed:\r\n" + publicKey.ToHex());

            string output = HashUtils.CreateBitcoinAddressVerbose(publicKey.Compress(true).ToBytes());
            Console.WriteLine(output);

            string check = HashUtils.WifToBitcoinAddress(wif);
            Console.WriteLine("Check:\r\n" + check);
        }
    }
}
