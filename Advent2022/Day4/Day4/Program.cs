using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day4
{
    public class Program
    {
        class AssignmentPair
        {
            long elf1_min;
            long elf1_max;
            long elf2_min;
            long elf2_max;

            public AssignmentPair(string s)
            {
                Match match = Regex.Match(s, @"^(?<elf1_min>\d+)-(?<elf1_max>\d+),(?<elf2_min>\d+)-(?<elf2_max>\d+)$");
                elf1_min = long.Parse(match.Groups["elf1_min"].Value);
                elf1_max = long.Parse(match.Groups["elf1_max"].Value);
                elf2_min = long.Parse(match.Groups["elf2_min"].Value);
                elf2_max = long.Parse(match.Groups["elf2_max"].Value);
            }

            public bool hasFullyContained()
            {
                return (elf1_min <= elf2_min && elf1_max >= elf2_max) || (elf2_min <= elf1_min && elf2_max >= elf1_max);
            }

            public bool hasOverlap()
            {
                return (elf1_min <= elf2_min && elf1_max >= elf2_min) || (elf2_min <= elf1_min && elf2_max >= elf1_min);
            }
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

        static IEnumerable<AssignmentPair> parse(List<string> input)
        {
            return input.Select(elem => new AssignmentPair(elem));
        }

        public static long part1(string file_name)
        {
            return parse(readInput(file_name)).Count(elem => elem.hasFullyContained());
        }

        public static long part2(string file_name)
        {
            return parse(readInput(file_name)).Count(elem => elem.hasOverlap());
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
