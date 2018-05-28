using System;
using System.Collections.Generic;
using System.Linq;

namespace _03.TheGreatSamuraiBattle
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] input = Console.ReadLine().Split(' ');
            if (string.IsNullOrWhiteSpace(input[0]))
            {
                return;
            }

            List<int> samurais = input
                .Select(v => int.Parse(v))
                .ToList();

            if (samurais.Count == 1)
            {
                return;
            }

            bool[] losers = new bool[samurais.Count];

            while (true)
            {
                for (int attacker = 0; attacker < samurais.Count; attacker++)
                {
                    if (!losers[attacker])
                    {
                        int target = samurais[attacker] % samurais.Count;
                        int battleResult = Math.Abs(attacker - target);
                        if (battleResult == 0)
                        {
                            losers[attacker] = true;
                            Console.WriteLine("{0} performed harakiri", attacker);
                        }
                        else if (battleResult % 2 == 1)
                        {
                            losers[attacker] = true;
                            Console.WriteLine("{0} x {1} -> {1} wins", attacker, target);
                        }
                        else
                        {
                            losers[target] = true;
                            Console.WriteLine("{0} x {1} -> {0} wins", attacker, target);
                        }

                        if (losers.Count(v => v == false) == 1)
                        {
                            return;
                        }
                    }
                }

                List<int> livingSamurais = new List<int>(samurais.Count);
                for (int i = 0; i < samurais.Count; i++)
                {
                    if (!losers[i])
                    {
                        livingSamurais.Add(samurais[i]);
                    }
                }
                samurais = livingSamurais;
                losers = new bool[samurais.Count];
            }
        }
    }
}
