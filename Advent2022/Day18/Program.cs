using static Common.Common;

List<(int x, int y, int z)> dirs = new List<(int x, int y, int z)>{ (1, 0, 0), (0, 1, 0), (0, 0, 1), (-1, 0, 0), (0, -1, 0), (0, 0, -1) };
(int x, int y, int z) Add((int x, int y, int z) p1, (int x, int y, int z) p2) => (p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);

(int x, int y, int z) min_point;
(int x, int y, int z) max_point;

bool inBounds((int x, int y, int z) point) => min_point.x <= point.x && point.x <= max_point.x && min_point.y <= point.y && point.y <= max_point.y && min_point.z <= point.z && point.z <= max_point.z;

void part1(string file_name)
{
    List<string> input = readInput(file_name);
    HashSet<(int x, int y, int z)> cubes = input.SelectMany(_ => _.Split(",")).Select(int.Parse).Chunk(3).Select(_ => (_[0], _[1], _[2])).ToHashSet();
    long connected_count = cubes.Select(_ => dirs.Select(d => Add(d, _)).Where(cubes.Contains).Count()).Sum();
    Console.WriteLine(input.Count * 6 - connected_count);
}

void part2(string file_name)
{
    HashSet<(int x, int y, int z)> cubes = readInput(file_name).SelectMany(_ => _.Split(",")).Select(int.Parse).Chunk(3).Select(_ => (_[0], _[1], _[2])).ToHashSet();
    min_point = (cubes.Min(_ => _.x) - 1, cubes.Min(_ => _.y) - 1, cubes.Min(_ => _.z) - 1);
    max_point = (cubes.Max(_ => _.x) + 1, cubes.Max(_ => _.y) + 1, cubes.Max(_ => _.z) + 1);
    HashSet<(int x, int y, int z)> steam = new();
    Stack<(int x, int y, int z)> frontier = new();
    frontier.Push(min_point);
    while(frontier.TryPop(out var curr))
    {
        if(!steam.Contains(curr) && !cubes.Contains(curr) && inBounds(curr))
        {
            steam.Add(curr);
            dirs.ForEach(d => frontier.Push(Add(d, curr)));
        }
    }
    Console.WriteLine(cubes.Select(_ => dirs.Select(d => Add(d, _)).Where(steam.Contains).Count()).Sum());
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");