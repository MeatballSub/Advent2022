using Common;
using System.Drawing;
using static Common.Common;

HashSet<Point> getRockStructures(string file_name)
{
    List<string> input = readInput(file_name);
    HashSet<Point> rock_structures = new HashSet<Point>();
    foreach (string i in input)
    {
        List<Point> points = new List<Point>();

        foreach (string p in i.Split(" -> "))
        {
            string[] coordinates = p.Split(',');
            Point point = new Point(int.Parse(coordinates.First()), int.Parse(coordinates.Last()));
            points.Add(point);
        }

        Point curr = points.FirstOrDefault(new Point(0, 0));
        foreach (Point p in points.Skip(1))
        {
            while (curr != p)
            {
                rock_structures.Add(curr);
                curr.X += Math.Sign(p.X - curr.X);
                curr.Y += Math.Sign(p.Y - curr.Y);
            }
            rock_structures.Add(curr);
        }
    }
    return rock_structures;
}

(bool, Point) simFall(Point sand, HashSet<Point> rock_structures, int floor_y = 0)
{
    bool resting = false;

    if (sand.Y == floor_y - 1)
    {
        resting = true;
    }
    else if (!rock_structures.Contains(new Point(sand.X, sand.Y + 1)))
    {
        ++sand.Y;
    }
    else if (!rock_structures.Contains(new Point(sand.X - 1, sand.Y + 1)))
    {
        --sand.X;
        ++sand.Y;
    }
    else if (!rock_structures.Contains(new Point(sand.X + 1, sand.Y + 1)))
    {
        ++sand.X;
        ++sand.Y;
    }
    else
    {
        resting = true;
    }
    return (resting, sand);
}

void part1(string file_name)
{
    HashSet<Point> rock_structures = getRockStructures(file_name);

    int min_x = rock_structures.Select(p => p.X).Min();
    int max_y = rock_structures.Select(p => p.Y).Max();

    bool abyss = false;
    long resting_sand = 0;
    while (!abyss)
    {
        Point sand = new Point(500, 0);
        bool resting = false;
        while (!resting && !abyss)
        {
            abyss = (sand.X < min_x || sand.Y >= max_y);
            if (!abyss)
            {
                (resting, sand) = simFall(sand, rock_structures);
            }
        }
        if (resting)
        {
            ++resting_sand;
            rock_structures.Add(sand);
        }
    }
    Console.WriteLine(resting_sand);
}

void part2(string file_name)
{
    HashSet<Point> rock_structures = getRockStructures(file_name);

    int max_y = rock_structures.Select(p => p.Y).Max();

    bool blocked = false;
    long resting_sand = 0;
    while (!blocked)
    {
        Point sand = new Point(500, 0);
        bool resting = false;
        while (!resting)
        {
            (resting, sand) = simFall(sand, rock_structures, max_y + 2);
        }
        if (resting)
        {
            ++resting_sand;
            rock_structures.Add(sand);
            blocked = resting && (sand.X == 500 && sand.Y == 0);
        }
    }
    Console.WriteLine(resting_sand);
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");