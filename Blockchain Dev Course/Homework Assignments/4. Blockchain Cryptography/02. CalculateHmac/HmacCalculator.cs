using Common;
using System;

namespace CalculateHmac
{
    internal class HmacCalculator
    {
        static void Main(string[] args)
        {
            Console.WriteLine(
                "HMAC-SHA-512{0}{1}",
                Environment.NewLine,
                HashUtils.ComputeHmac(
                    "HMACSHA512", 
                    Utils.GetBytes("blockchain"), 
                    Utils.GetBytes("devcamp")));
        }
    }
}
