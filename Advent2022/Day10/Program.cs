using static Common.Common;

List<int> parse(string file_name)
{
    List<int> x_vals = new List<int>() { 1 };

    foreach (string command in readInput(file_name))
    {
        x_vals.Add(x_vals.Last());
        if (command != "noop")
        {
            int v = int.Parse(command.Split(' ').Last());
            x_vals.Add(x_vals.Last() + v);
        }
    }

    return x_vals;
}

void part1(string file_name)
{
    List<int> x_vals = parse(file_name);
    long strength = Enumerable.Range(0, 6).Select(i => i * 40 + 20).Select(i => x_vals[i - 1] * i).Sum();
    Console.WriteLine(strength);
    Console.WriteLine();
}

void part2(string file_name)
{
    List<int> x_vals = parse(file_name);
    for (int y = 0; y < 6; ++y)
    {
        for (int x = 0; x < 40; ++x)
        {
            int index = y * 40 + x;
            Console.Write(Enumerable.Range(x_vals[index] - 1, 3).Contains(x) ? '#' : ' ');
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");