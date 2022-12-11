using System.Text.RegularExpressions;
using static Common.Common;

int getStackCount(string stack_labels)
{
    return int.Parse(stack_labels.Split(' ', StringSplitOptions.RemoveEmptyEntries).Last());
}

List<Stack<char>> allocateStacks(int count)
{
    List<Stack<char>> stacks = new List<Stack<char>>();

    for (int i = 0; i < count; ++i)
    {
        stacks.Add(new Stack<char>());
    }

    return stacks;
}

void fillStacks(Stack<string> stack_input, List<Stack<char>> stacks)
{
    while (stack_input.Count > 0)
    {
        string stack_line = stack_input.Pop();

        for (int i = 0; i < stacks.Count; ++i)
        {
            int crate_index = (i * 4) + 1;
            char crate_label = stack_line[crate_index];
            if (crate_label != ' ')
            {
                stacks[i].Push(crate_label);
            }
        }
    }
}

List<Stack<char>> initStacks(Stack<string> stack_input)
{
    int stack_count = getStackCount(stack_input.Pop());
    List<Stack<char>> stacks = allocateStacks(stack_count);
    fillStacks(stack_input, stacks);
    return stacks;
}

IEnumerable<(int, int, int)> initMoves(List<string> move_input)
{
    return move_input
        .Select(move => Regex.Match(move, @"move (?<count>\d+) from (?<from>\d+) to (?<to>\d+)"))
        .Select(m => (int.Parse(m.Groups["count"].Value), int.Parse(m.Groups["from"].Value), int.Parse(m.Groups["to"].Value)));
}

(List<Stack<char>> stacks, IEnumerable<(int, int, int)> moves) parse(List<string> input)
{
    Stack<string> stack_input = new Stack<string>();
    List<string> move_input = new List<string>();

    bool processing_stacks = true;

    foreach (string s in input)
    {
        bool not_blank_line = !string.IsNullOrWhiteSpace(s);
        processing_stacks = processing_stacks && not_blank_line;

        if (processing_stacks)
        {
            stack_input.Push(s);
        }
        else if (not_blank_line)
        {
            move_input.Add(s);
        }
    }

    List<Stack<char>> stacks = initStacks(stack_input);
    IEnumerable<(int, int, int)> moves = initMoves(move_input);

    return (stacks, moves);
}

void applyMoves(IEnumerable<(int, int, int)> moves, List<Stack<char>> stacks)
{
    foreach (var (count, from, to) in moves)
    {
        for(int i = 0; i < count; ++i)
        {
            stacks[to - 1].Push(stacks[from - 1].Pop());
        }
    }
}

void applyMultiMoves(IEnumerable<(int, int, int)> moves, List<Stack<char>> stacks)
{
    foreach (var (count, from, to) in moves)
    {
        Stack<char> temp = new Stack<char>();

        for (int i = 0; i < count; ++i)
        {
            temp.Push(stacks[from - 1].Pop());
        }

        for (int i = 0; i < count; ++i)
        {
            stacks[to - 1].Push(temp.Pop());
        }
    }
}

void part1(string file_name)
{
    var (stacks, moves) = parse(readInput(file_name));
    applyMoves(moves, stacks);
    Console.WriteLine(stacks.Select(stack => stack.Pop()).ToArray());
}

void part2(string file_name)
{
    var (stacks, moves) = parse(readInput(file_name));
    applyMultiMoves(moves, stacks);
    Console.WriteLine(stacks.Select(stack => stack.Pop()).ToArray());
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");