using System.Drawing;
using static Common.Common;

IEnumerable<(string, int)> parse(string file_name)
{
    return readInput(file_name).Select(s => (s.Split(' ').First(), int.Parse(s.Split(' ').Last())));
}

Point updateHead(string direction, Point head) => direction switch
{
    "U" => new Point(head.X, head.Y - 1),
    "D" => new Point(head.X, head.Y + 1),
    "L" => new Point(head.X - 1, head.Y),
    "R" => new Point(head.X + 1, head.Y),
    _ => head,
};

int follow(int leader, int follower) => follower + Math.Sign(leader - follower);

Point updateTail(Point head, Point tail)
{
    int delta_x = head.X - tail.X;
    int delta_y = head.Y - tail.Y;
    int distance = (delta_x * delta_x) + (delta_y * delta_y);
    return (distance > 2) ? new Point(follow(head.X, tail.X), follow(head.Y, tail.Y)) : tail;
}

HashSet<Point> applyMotions(IEnumerable<(string direction, int distance)> motions, int knot_count)
{
    HashSet<Point> visited = new HashSet<Point>();
    const int head = 0;
    int tail = knot_count - 1;
    List<Point> knots = Enumerable.Repeat(new Point(0, 0), knot_count).ToList();

    visited.Add(knots[tail]);

    foreach (var (direction, distance) in motions)
    {
        for (int i = 0; i < distance; ++i)
        {
            knots[head] = updateHead(direction, knots[head]);
            foreach(int knot in Enumerable.Range(1, tail))
            {
                knots[knot] = updateTail(knots[knot - 1], knots[knot]);
            }
            visited.Add(knots[tail]);
        }
    }

    return visited;
}

void part1(string file_name)
{
    Console.WriteLine(applyMotions(parse(file_name), 2).Count());
}

void part2(string file_name)
{
    Console.WriteLine(applyMotions(parse(file_name), 10).Count());
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("sample_input2.txt");
part2("input.txt");