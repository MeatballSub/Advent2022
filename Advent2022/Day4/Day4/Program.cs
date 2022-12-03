using System;
using System.Collections.Generic;
using System.IO;

namespace Day4
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

        public static long part1(string file_name)
        {
            return 0;
        }

        public static long part2(string file_name)
        {
            return 0;
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
