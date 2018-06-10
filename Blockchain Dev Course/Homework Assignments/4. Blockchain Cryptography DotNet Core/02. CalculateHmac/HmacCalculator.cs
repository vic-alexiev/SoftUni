using Common;
using System;

namespace CalculateHmac
{
    internal class HmacCalculator
    {
        private static void Main(string[] args)
        {
            Console.WriteLine(
                "HMAC-SHA-512{0}{1}",
                Environment.NewLine,
                Utils.ToString(HashUtils.ComputeHmac(
                    Utils.GetBytes("blockchain"),
                    Utils.GetBytes("devcamp"),
                    512)));
        }
    }
}
