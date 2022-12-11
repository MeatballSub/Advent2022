using Common;
using static Common.Common;

bool isVisible(List<string> forest, int x, int y)
{
    var colCheck = (int row) => forest[row][x] < forest[y][x];
    var rowCheck = (int col) => forest[y][col] < forest[y][x];

    List<bool> visibility = new List<bool>()
    {
        Enumerable.Range(0, y).All(colCheck),                             // top
        Enumerable.Range(y + 1, forest.Count() - y - 1).All(colCheck),    // bottom
        Enumerable.Range(0, x).All(rowCheck),                             // left
        Enumerable.Range(x + 1, forest[y].Length - x - 1).All(rowCheck),  // right
    };
    return visibility.Any(_ => _);
}

long scenicScore(List<string> forest, int x, int y)
{
    var colCheck = (int row) => forest[row][x] >= forest[y][x];
    var rowCheck = (int col) => forest[y][col] >= forest[y][x];

    List<long> view_distances = new List<long>()
    {
        y - Enumerable.Range(0, y).Reverse().FirstOrDefault(colCheck, 0),                                     // up
        Enumerable.Range(y + 1, forest.Count() - y - 1).FirstOrDefault(colCheck, forest.Count() - 1) - y,     // down
        x - Enumerable.Range(0, x).Reverse().FirstOrDefault(rowCheck, 0),                                     // left
        Enumerable.Range(x + 1, forest[y].Length - x - 1).FirstOrDefault(rowCheck, forest[y].Length - 1) - x, // right
    };

    return view_distances.Product();
}

long execute(string file_name, Calculation calc)
{
    List<string> forest = readInput(file_name);
    int width = forest.First().Length;
    int height = forest.Count();
    return calc(forest, width, height, Enumerable.Range(0, width * height));
}

void part1(string file_name)
{
    Calculation visible_count = (forest, width, height, range) => range.Count(i => isVisible(forest, i % width, i / width));
    long count = execute(file_name, visible_count);
    Console.WriteLine(count);
}

void part2(string file_name)
{
    Calculation scenic_max = (forest, width, height, range) => range.Max(i => scenicScore(forest, i % width, i / width));
    long max = execute(file_name, scenic_max);
    Console.WriteLine(max);
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");

delegate long Calculation(List<string> forest, int width, int height, IEnumerable<int> range);