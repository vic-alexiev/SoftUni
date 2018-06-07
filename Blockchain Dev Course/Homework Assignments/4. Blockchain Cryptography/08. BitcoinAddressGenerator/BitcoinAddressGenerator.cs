using Common;
using Nethereum.Hex.HexConvertors.Extensions;
using System;

namespace BitcoinAddressGenerator
{
    internal class BitcoinAddressGenerator
    {
        private static void Main(string[] args)
        {
            string hexHash =
                "0450863AD64A87AE8A2FE83C1AF1A8403CB53F53E486D8511DAD8A04887E5B23522CD470243453A299FA9E77237716103ABC11A1DF38855ED6F2EE187E9C582BA6";
            CreateBitcoinAddressStepByStep(hexHash);

            //string btcAddress = HashUtils.CreateBitcoinAddress(hexHash);
            //Console.WriteLine("BTC address: " + btcAddress);
        }

        private static void CreateBitcoinAddressStepByStep(string hexHash)
        {
            byte[] step1 = hexHash.HexToByteArray();
            Console.WriteLine("1. public_key:\r\n" + step1.ToHex());

            byte[] step2_SHA256 = HashUtils.ComputeSha256(step1);
            Console.WriteLine("2. SHA256(public_key):\r\n" + step2_SHA256.ToHex());

            byte[] step3_RIPEMD160 = HashUtils.ComputeRIPEMD160(step2_SHA256);
            Console.WriteLine("3. RIPEMD160(SHA256(public_key)):\r\n" + step3_RIPEMD160.ToHex());

            byte[] step4_prepend_byte = ArrayUtils.ConcatArrays(new byte[1], step3_RIPEMD160);
            Console.WriteLine("4. [0x00] + RIPEMD160(SHA256(public_key)):\r\n" + step4_prepend_byte.ToHex());

            byte[] step5_SHA256 = HashUtils.ComputeSha256(step4_prepend_byte);
            Console.WriteLine("5. SHA256([0x00] + RIPEMD160(SHA256(public_key))):\r\n" + step5_SHA256.ToHex());

            byte[] step6_SHA256 = HashUtils.ComputeSha256(step5_SHA256);
            Console.WriteLine("6. SHA256(SHA256([0x00] + RIPEMD160(SHA256(public_key)))):\r\n" + step6_SHA256.ToHex());

            byte[] step7_checksum = ArrayUtils.SubArray(step6_SHA256, 0, 4);
            Console.WriteLine("7. checksum = First4Bytes(SHA256(SHA256([0x00] + RIPEMD160(SHA256(public_key))))):\r\n" + step7_checksum.ToHex());

            byte[] step8_BTC_address = ArrayUtils.ConcatArrays(step4_prepend_byte, step7_checksum);
            Console.WriteLine("8. BTC_address = [0x00] + RIPEMD160(SHA256(public_key)) + checksum:\r\n" + step8_BTC_address.ToHex());

            string step9_Base58Check_encode = Base58CheckEncoding.EncodePlain(step8_BTC_address);
            Console.WriteLine("9. Base58CheckEncode(BTC_address):\r\n" + step9_Base58Check_encode);
        }
    }
}
