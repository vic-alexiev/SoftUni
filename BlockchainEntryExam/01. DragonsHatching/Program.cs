using System;
using System.Linq;

namespace _01.DragonsHatching
{
    class Program
    {
        static void Main(string[] args)
        {
            int teamsCount = int.Parse(Console.ReadLine());
            decimal[] teamPoints = new decimal[teamsCount];

            decimal dragonHatchingValue = decimal.Parse(Console.ReadLine());

            for (int i = 0; i < teamsCount; i++)
            {
                string[] teamValues = Console.ReadLine().Split(' ');
                decimal dragonsHatched = decimal.Parse(teamValues[0]);
                int teamMembersCount = int.Parse(teamValues[1]);

                teamPoints[i] = dragonsHatched / teamMembersCount;
            }

            decimal sum = teamPoints.Sum();
            if (dragonHatchingValue == Decimal.Zero)
            {
                Console.WriteLine(sum.ToString("N3").Replace(",", ""));
            }
            else
            {
                Console.WriteLine((sum / dragonHatchingValue).ToString("N3").Replace(",", ""));
            }
        }
    }
}
