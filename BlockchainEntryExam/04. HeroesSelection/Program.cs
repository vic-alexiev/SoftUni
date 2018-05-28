using System;
using System.Collections.Generic;
using System.Linq;

namespace _04.HeroesSelection
{
    class Program
    {
        private static SortedDictionary<string, Dictionary<string, int>> heroes =
            new SortedDictionary<string, Dictionary<string, int>>();
        static void Main(string[] args)
        {

            string input = Console.ReadLine();
            while (input != "Make a decision already!")
            {
                string[] data = input.Split(' ');
                string name = data[0];

                if (input.Contains("Gyubek"))
                {
                    if (heroes.ContainsKey(name))
                    {
                        heroes[name] = new Dictionary<string, int>();
                    }
                }
                else
                {
                    string trait = data[1];
                    int value = int.Parse(data[2]);

                    if (!heroes.ContainsKey(name))
                    {
                        heroes.Add(name, new Dictionary<string, int>());
                    }

                    int traitValue = 0;
                    if (trait == "Greedy" ||
                        trait == "Rude" ||
                        trait == "Dumb")
                    {
                        traitValue = -value;
                    }
                    else if (trait == "Kind")
                    {
                        traitValue = value * 2;
                    }
                    else if (trait == "Handsome")
                    {
                        traitValue = value * 3;
                    }
                    else if (trait == "Smart")
                    {
                        traitValue = value * 5;
                    }
                    else
                    {
                        traitValue = value;
                    }

                    if (!heroes[name].ContainsKey(trait))
                    {
                        heroes[name].Add(trait, traitValue);
                    }
                    else if (traitValue > heroes[name][trait])
                    {
                        heroes[name][trait] = traitValue;
                    }
                }

                input = Console.ReadLine();
            }

            var totals = new SortedDictionary<int, List<string>>(new DescendingComparer<int>());
            var traits = new SortedDictionary<string, SortedDictionary<int, List<string>>>();

            foreach (var hero in heroes)
            {
                int sum = hero.Value.Values.Sum();
                if (!totals.ContainsKey(sum))
                {
                    totals.Add(sum, new List<string>());
                }
                totals[sum].Add(hero.Key);

                if (!traits.ContainsKey(hero.Key))
                {
                    traits.Add(
                        hero.Key,
                        new SortedDictionary<int, List<string>>(new DescendingComparer<int>()));

                    foreach (var trait in hero.Value)
                    {
                        if (!traits[hero.Key].ContainsKey(trait.Value))
                        {
                            traits[hero.Key].Add(trait.Value, new List<string>());
                        }
                        traits[hero.Key][trait.Value].Add(trait.Key);
                    }
                }
            }

            foreach (var total in totals)
            {
                foreach (var name in total.Value)
                {
                    Console.WriteLine("# {0}: {1}", name, total.Key);
                    foreach (var trait in traits[name])
                    {
                        foreach (var traitName in trait.Value)
                        {
                            Console.WriteLine("!!! {0} -> {1}", traitName, trait.Key);
                        }
                    }
                }
            }
        }

        private class DescendingComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y)
            {
                return y.CompareTo(x);
            }
        }
    }
}
