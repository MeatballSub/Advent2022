using System.Drawing;
using static Common.Common;

void part1(string file_name)
{
    Puzzle puzzle = new Puzzle(file_name);
    Console.WriteLine(puzzle.heightAfterPieces(2022));
}

void part2(string file_name)
{
    Puzzle puzzle = new Puzzle(file_name);
    Console.WriteLine(puzzle.heightAfterPieces(1000000000000));
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");

class History
{
    public int length { get; init; }
    List<Size> history = new();

    public History(int length)
    {
        this.length = length;
    }

    public void Add(Size p)
    {
        history.Add(p);
        history = history.TakeLast(length).ToList();
    }

    public override bool Equals(object? obj) => Equals(obj as History);

    public bool Equals(History? other) => other != null && history.SequenceEqual(other.history);

    public override int GetHashCode() => history.Select(_ => _.GetHashCode()).Aggregate(0, (a, b) => HashCode.Combine(a, b));
}

class Puzzle
{
    public string jet_pattern { get; set; }
    public List<HashSet<Point>> shapes { get; set; }
    public HashSet<Point> board { get; set; }

    private int jet_index = 0;
    private int shape_index = 0;

    private History history;

    public Puzzle(string file_name)
    {
        string shape_definitions = @"
####

.#.
###
.#.

..#
..#
###

#
#
#
#

##
##";

        jet_pattern = readInput(file_name).First();
        shapes = new();
        foreach (string shape in shape_definitions.Split("\r\n\r\n"))
        {
            HashSet<Point> sprite = new();
            string[] rows = shape.Split("\r\n");
            for (int y = 0; y < rows.Length; ++y)
            {
                for (int x = 0; x < rows[y].Length; ++x)
                {
                    if (rows[y][x] == '#')
                    {
                        sprite.Add(new Point(x, rows.Length - y - 1));
                    }
                }
            }
            shapes.Add(sprite);
        }
        board = Enumerable.Range(0, 7).Select(_ => new Point(_, 0)).ToHashSet();

        // this value is arbitrary, larger is more accurate, smaller is faster
        // at 2, it got wrong answers, so that's too small, 3 worked but doesn't make a lot of sense
        // Just keep the history of the last cycle of shapes seems reasonable
        history = new(shapes.Count);
    }

    public long heightAfterPieces(long num_pieces)
    {
        Dictionary<(int, int, History), (long, long)> period = new();
        bool period_found = false;
        long period_bonus = 0;

        for (long i = 0; i < num_pieces; ++i)
        {
            if (!period_found && i >= history.length)
            {
                var key = (jet_index, shape_index, history);
                var value = (i, boardHeight());
                if (period.ContainsKey(key))
                {
                    period_found = true;
                    var (historic_fallen, historic_height) = period[key];
                    long pieces_per_period = i - historic_fallen;
                    long height_per_period = boardHeight() - historic_height;
                    long periods_left = (num_pieces - i) / pieces_per_period;
                    i += periods_left * pieces_per_period;
                    period_bonus = periods_left * height_per_period;
                }
                else
                {
                    period.Add(key, value);
                }
            }
            if (i < num_pieces)
            {
                simFall();
                nextShape();
            }
        }
        return boardHeight() + period_bonus;
    }

    private long boardHeight() => board.Select(_ => _.Y).Max();

    private bool collision(Point offset) => shapes[shape_index].Select(_ => _ + new Size(offset)).Any(_ => board.Contains(_) || _.X < 0 || _.X > 6);

    private void land(Point offset) => board.UnionWith(shapes[shape_index].Select(_ => _ + new Size(offset)));

    private void nextJet() => jet_index = (jet_index + 1) % jet_pattern.Length;

    private void nextShape() => shape_index = (shape_index + 1) % shapes.Count;

    private void simFall()
    {
        Size down = new Size(0, -1);
        Size left = new Size(-1, 0);
        Size right = new Size(1, 0);

        Size start_offset = new Size(2, board.Select(_ => _.Y).Max() + 4);
        Point offset = new Point(start_offset);

        while (true)
        {
            Size jet_move = jet_pattern[jet_index] == '<' ? left : right;

            nextJet();

            if (!collision(offset + jet_move))
            {
                offset += jet_move;
            }

            if (collision(offset + down))
            {
                land(offset);
                break;
            }
            else
            {
                offset += down;
            }
        }

        Size end_offset = new Size(offset);
        history.Add(end_offset - start_offset);
    }

}