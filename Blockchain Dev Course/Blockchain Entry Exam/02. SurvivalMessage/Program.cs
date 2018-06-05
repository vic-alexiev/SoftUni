using System;
using System.Collections.Generic;
using System.Linq;

namespace _02.SurvivalMessage
{
    class Program
    {
        private static int[] validNs = { 12, 22, 32, 42, 52, 62, 72, 82, 92 };
        private static int[] validMs = { 15, 25, 35, 45, 55, 65, 75, 85, 95 };
        private static string validUppers = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string validLowers = "abcdefghijklmnopqrstuvwxyz";

        static void Main(string[] args)
        {
            int N = int.Parse(Console.ReadLine());
            char upper1 = char.Parse(Console.ReadLine());
            char lower = char.Parse(Console.ReadLine());
            char upper2 = char.Parse(Console.ReadLine());
            int M = int.Parse(Console.ReadLine());
            int count = int.Parse(Console.ReadLine());

            int generatedCount = 0;

            foreach (int n in GetNs(N))
            {
                foreach (char u1 in GetUppers(upper1))
                {
                    foreach (char low in GetLowers(lower))
                    {
                        foreach (char u2 in GetUppers(upper2))
                        {
                            foreach (int m in GetMs(M))
                            {
                                generatedCount++;
                                if (generatedCount == count)
                                {
                                    Console.WriteLine("{0}{1}{2}{3}{4}", n, u1, low, u2, m);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<int> GetNs(int N)
        {
            return validNs.Where(n => n >= N);
        }

        private static List<int> GetMs(int M)
        {
            List<int> Ms = new List<int>();
            for (int i = validMs.Length - 1; i >= 0; i--)
            {
                if (validMs[i] <= M)
                {
                    Ms.Add(validMs[i]);
                }
            }
            return Ms;
        }

        private static IEnumerable<char> GetUppers(char upper)
        {
            return validUppers.Where(c => c >= upper);
        }

        private static IEnumerable<char> GetLowers(char lower)
        {
            return validLowers.Where(c => c >= lower);
        }
    }
}
