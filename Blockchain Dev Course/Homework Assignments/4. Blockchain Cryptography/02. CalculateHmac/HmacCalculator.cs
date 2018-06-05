using Common;
using System;
using System.Security.Cryptography;

namespace CalculateHmac
{
    internal class HmacCalculator
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "HMAC-SHA-512{0}{1}",
                Environment.NewLine,
                CalcHmac("HMACSHA512", "blockchain", "devcamp"));
        }

        private static string CalcHmac(string algorithmName, string message, string key)
        {
            HMAC hmac = HMAC.Create(algorithmName);
            hmac.Key = Utils.GetBytes(key);
            byte[] data = hmac.ComputeHash(Utils.GetBytes(message));
            return Utils.ToString(data);
        }
    }
}
