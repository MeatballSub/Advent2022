using Common;
using System.Collections.Immutable;
using System.Drawing;
using static BFSGraph;
using static Common.Common;

int stepsFromTo(Node goal, Point start)
{
    int step_count = 0;
    if (goal != null)
    {
        while (goal.Location != start)
        {
            ++step_count;
            goal = goal.Parent;
        }
    }
    return step_count;
}

void part1(string file_name)
{
    BFSGraph graph = new BFSGraph(readInput(file_name));
    BFSGraph.Node? goal = graph.findPath();
    int step_count = stepsFromTo(goal, graph.Start);
    Console.WriteLine(step_count);
}

void part2(string file_name)
{
    BFSGraph graph = new BFSGraph(readInput(file_name));
    BFSGraph.Node? goal = graph.findBestHikingTrail();
    int step_count = stepsFromTo(goal, graph.End);
    Console.WriteLine(step_count);
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");

public class BFSGraph
{
    delegate bool GoalTest(Point location);
    delegate bool ValidateNeighbor(Point curr, Point neighbor);

    public Point Start { get; set; }
    public Point End{ get; set; }
    List<string> map;
    int width;
    int height;

    ImmutableList<Point> offsets = ImmutableList.Create(
        new Point(0, -1),
        new Point(0, 1),
        new Point(-1, 0),
        new Point(1, 0)
    );

    public class Node
    {
        public Point Location { get; set; }
        public Node? Parent { get; set; }

        public Node(Point location, Node? parent = null)
        {
            this.Location = location;
            this.Parent = parent;
        }
    }

    public BFSGraph(List<string> map)
    {
        this.map = map;
        height = map.Count;
        width = map[0].Length;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                if (map[y][x] == 'S')
                {
                    Start = new Point(x, y);
                    map[y] = map[y].Replace('S','a');
                }
                else if (map[y][x] == 'E')
                {
                    End = new Point(x, y);
                    map[y] = map[y].Replace('E', 'z');
                }
            }
        }
    }

    private List<Point> getNeighbors(Point location)
    {
        List<Point> neighbors = new List<Point>();
        foreach (Point offset in offsets)
        {
            Point neighbor = offset;
            neighbor.Offset(location);
            if (neighbor.X >= 0 && neighbor.X < width && neighbor.Y >= 0 && neighbor.Y < height)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    private Node? search(Point initial_position, GoalTest goalTest, ValidateNeighbor validateNeighbor)
    {
        HashSet<Point> explored = new HashSet<Point>();
        Queue<Node> frontier = new Queue<Node>();
        explored.Add(initial_position);
        frontier.Enqueue(new Node(initial_position));
        while (frontier.Count > 0)
        {
            Node node = frontier.Dequeue();
            if (goalTest(node.Location))
            {
                return node;
            }
            foreach(Point neighbor in getNeighbors(node.Location))
            {
                if(validateNeighbor(node.Location, neighbor) && !explored.Contains(neighbor))
                {
                    explored.Add(neighbor);
                    frontier.Enqueue(new Node(neighbor, node));
                }
            }
        }
        return null;
    }

    public Node? findPath()
    {
        return search(Start, loc => loc == End, (curr, next) => (map[next.Y][next.X] - map[curr.Y][curr.X] < 2));
    }

    public Node? findBestHikingTrail()
    {
        return search(End, loc => (map[loc.Y][loc.X] == 'a'), (curr, next) => (map[curr.Y][curr.X] - map[next.Y][next.X] < 2));
    }
}