using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day2
{
    public class Program
    {
        public enum Part
        {
            part1,
            part2,
        }
        public class RPSRound
        {
            public const int opponent_index = 0;
            public const int you_index = 2;
            public const char opponent_start_char = 'A';
            public const char you_start_char = 'X';
            public const char you_tie_char = 'Y';

            enum Shape
            {
                Rock,
                Paper,
                Scissors,
                Count,
            }

            Shape opponent;
            Shape you;

            public RPSRound(string s, Part part)
            {
                opponent = (Shape)(s[opponent_index] - opponent_start_char);

                if (part == Part.part1)
                {
                    you = (Shape)(s[you_index] - you_start_char);
                }
                else if (part == Part.part2)
                {
                    you = (Shape)(((int)opponent + (s[you_index] - you_tie_char)) % (int)Shape.Count);
                }
                else
                {
                    // weird part, go all rock!
                    you = Shape.Rock;
                }
            }

            int shapeScore()
            {
                if (you == Shape.Rock) return 1;
                if (you == Shape.Paper) return 2;
                return 3;
            }

            int outcomeScore()
            {
                if(opponent == you) return 3;
                if (you == (Shape)(((int)opponent + 1) % (int)Shape.Count)) return 6;
                return 0;
            }

            public long score()
            {
                return shapeScore() + outcomeScore();
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

        static IEnumerable<RPSRound> parse(List<string> input, Part part)
        {
            return input.Select(x => new RPSRound(x, part));
        }

        public static long part1(string file_name)
        {
            return parse(readInput(file_name), Part.part1).Sum(x => x.score());
        }

        public static long part2(string file_name)
        {
            return parse(readInput(file_name), Part.part2).Sum(x => x.score());
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
