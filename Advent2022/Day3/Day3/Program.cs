using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day3
{
    public class Program
    {
        public class Rucksack
        {
            char[] items;
            char[] first_compartment;
            char[] second_compartment;

            public Rucksack(string s)
            {
                items = s.ToCharArray();
                first_compartment = s.Substring(0, s.Length / 2).ToCharArray();
                second_compartment = s.Substring(s.Length / 2).ToCharArray();
            }

            public char intersect() => first_compartment.Intersect(second_compartment).First();

            public IEnumerable<char> getItems() => items;
        }

        static int charToPriority(char c)
        {
            if (Char.IsLower(c)) return c - 'a' + 1;
            return c - 'A' + 27;
        }

        static char getBadge(IEnumerable<Rucksack> rucksacks)
        {
            return rucksacks.Aggregate(rucksacks.First().getItems(), (agg, ele) => agg.Intersect(ele.getItems())).First();
        }

        static List<string> readInput(string file_name)
        {
            List<string> input = new List<string>();
            using (TextReader reader = File.OpenText(file_name))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    input.Add(line);
                }
            }
            return input;
        }

        static IEnumerable<Rucksack> parse(List<string> input)
        {
            return input.Select(x => new Rucksack(x));
        }

        public static long part1(string file_name)
        {
            return parse(readInput(file_name)).Sum(x => charToPriority(x.intersect()));
        }

        public static long part2(string file_name)
        {
            return parse(readInput(file_name))
                .Select((rucksack, index) => new { rucksack, index })
                .GroupBy(g => g.index / 3, i => i.rucksack, (key, rucksacks) => getBadge(rucksacks))
                .Sum(x => charToPriority(x));
        }

        static void Main(string[] args)
        {
            Console.WriteLine(part1("sample_input.txt"));
            Console.WriteLine(part1("input.txt"));
            Console.WriteLine(part2("sample_input.txt"));
            Console.WriteLine(part2("input.txt"));
        }
    }
}
