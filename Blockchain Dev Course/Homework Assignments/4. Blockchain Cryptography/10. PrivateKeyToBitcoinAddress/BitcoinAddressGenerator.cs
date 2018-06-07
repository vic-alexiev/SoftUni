using Common;
using NBitcoin;
using System;

namespace PrivateKeyToBitcoinAddress
{
    class BitcoinAddressGenerator
    {
        static void Main(string[] args)
        {
            string wif = "KxyJg5ZCk6SFsorqiR7Bb6heDDmss4PX76VtyM5qTEyn83Bgt1tD";
            Key privateKey = Key.Parse(wif);
            PubKey publicKey = privateKey.PubKey;
            BitcoinPubKeyAddress bitcoinAddress = publicKey.GetAddress(Network.Main);
            Console.WriteLine(bitcoinAddress);

            string a = HashUtils.CreateBitcoinAddress(publicKey.Compress().ToHex());
            Console.WriteLine(bitcoinAddress);
        }
    }
}
