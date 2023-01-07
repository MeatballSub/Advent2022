using static Common.Common;

Node parse(string file_name)
{
    IEnumerable<string[]> input = readInput(file_name).Select(_ => _.Split(new char[] { ':', ' ' }, StringSplitOptions.RemoveEmptyEntries));
    Dictionary<string, Node> expressions = input.Select(_ => new Node(_)).ToDictionary(_ => _.Name, _ => _);
    var node = (string[] line) => expressions[line[0]];
    var left = (string[] line) => expressions[line[1]];
    var right = (string[] line) => expressions[line[3]];
    input.Where(_ => Node.isExpression(_)).ToList().ForEach(_ => { Node n = node(_); n.Left = left(_); n.Right = right(_); });
    return expressions["root"];
}

(Node, Node, bool) nextSubtree(Node humn_root)
{
    bool humn_is_left = humn_root.Left.ContainsHumn();
    Node new_humn_root = humn_is_left ? humn_root.Left : humn_root.Right;
    Node other_root = humn_is_left ? humn_root.Right : humn_root.Left;
    return (new_humn_root, other_root, humn_is_left);
}

void part1(string file_name)
{
    Node root = parse(file_name);
    Console.WriteLine(root.eval());
}

void part2(string file_name)
{
    Node root = parse(file_name);
    var (humn_root, other_root, humn_is_left) = nextSubtree(root);
    Node eval_root = other_root;

    while (humn_root.Name != "humn")
    {
        string op = humn_root.Op;
        (humn_root, other_root, humn_is_left) = nextSubtree(humn_root);
        eval_root = op switch
        {
            "+" => new Node(eval_root, "-", other_root),
            "-" => humn_is_left ? new Node(eval_root, "+", other_root) : new Node(other_root, "-", eval_root),
            "*" => new Node(eval_root, "/", other_root),
            "/" => humn_is_left ? new Node(eval_root, "*", other_root) : new Node(other_root, "/", eval_root),
            _ => throw new NotImplementedException()
        };
    }

    Console.WriteLine(eval_root.eval());
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");

class Node
{
    private const int name_index = 0;
    private const int value_index = 1;
    private const int op_index = 2;
    private bool contains_humn = false;
    private long value = 0;

    public string Name { get; set; } = string.Empty;
    public Node? Left { get; set; } = null;
    public Node? Right { get; set; } = null;
    public string Op { get; set; } = string.Empty;

    public bool ContainsHumn()
    {
        if (Left != null && Right != null)
        {
            contains_humn = Left.ContainsHumn() || Right.ContainsHumn();
        }
        return contains_humn;
    }

    public static bool isExpression(string[] properties) => properties.Length > op_index;

    public Node(Node left, string op, Node right)
    {
        this.Left = left;
        this.Op = op;
        this.Right = right;
    }

    public Node(string[] properties)
    {
        Name = properties[name_index];
        if (isExpression(properties))
        {
            Op = properties[op_index];
        }
        else
        {
            value = long.Parse(properties[value_index]);
            contains_humn = (Name == "humn");
        }
    }

    public long eval()
    {
        if (Left != null && Right != null)
        {
            value = Op switch
            {
                "+" => Left.eval() + Right.eval(),
                "-" => Left.eval() - Right.eval(),
                "*" => Left.eval() * Right.eval(),
                "/" => Left.eval() / Right.eval(),
                _ => throw new NotImplementedException()
            };
        }
        return value;
    }
}