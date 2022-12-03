using System.Collections.Generic;
using Xunit;

namespace Day3Test
{
    public class UnitTest1
    {
        [Theory]
        [MemberData(nameof(Part1sample))]
        [MemberData(nameof(Part1))]
        public void Part1Test(string file_name, int answer)
        {
            long result = Day3.Program.part1(file_name);
            Assert.Equal(answer, result);
        }

        [Theory]
        [MemberData(nameof(Part2sample))]
        [MemberData(nameof(Part2))]
        public void Part2Test(string file_name, long answer)
        {
            long result = Day3.Program.part2(file_name);
            Assert.Equal(answer, result);
        }

        public static IEnumerable<object[]> TestSetup(string file_name, long answer)
        {
            return new[]
            {
                new object[] {file_name, answer}
            };
        }

        public static IEnumerable<object[]> Part1sample => TestSetup("sample_input.txt", 157);

        public static IEnumerable<object[]> Part1 => TestSetup("input.txt", 8105);

        public static IEnumerable<object[]> Part2sample => TestSetup("sample_input.txt", 70);

        public static IEnumerable<object[]> Part2 => TestSetup("input.txt", 2363);
    }
}