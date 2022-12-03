using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day1
{
    public class Program
    {
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

        static List<long> parse(List<string> input)
        {
            List<long> sums = new List<long>();
            long sum = 0;
            foreach(string s in input)
            {
                if(s != string.Empty)
                {
                    sum += long.Parse(s);
                }
                else
                {
                    sums.Add(sum);
                    sum = 0;
                }
            }
            return sums;
        }

        static IEnumerable<long> getKLargest(List<long> list, int k)
        {
            return list.OrderByDescending(x => x).Take(k);
        }

        public static long part1(string file_name)
        {
            List<long> sums = parse(readInput(file_name));
            IEnumerable<long> k_largest = getKLargest(sums, 1);
            return k_largest.Sum();
        }

        public static long part2(string file_name)
        {
            List<long> sums = parse(readInput(file_name));
            IEnumerable<long> k_largest = getKLargest(sums, 3);
            return k_largest.Sum();
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
