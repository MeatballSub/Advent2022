using System.Text.RegularExpressions;
using Common;
using static Common.Common;

List<Monkey> parse(string file_name)
{
    List<string> input = readInput(file_name);
    int num_monkeys = (input.Count() + 1) / 7;
    return Enumerable.Range(0, num_monkeys).Select(i => input.Skip(i * 7).Take(6)).Select(list => new Monkey(list.ToList())).ToList();
}

void processRound(List<Monkey> monkeys, Relief relief, long modulus)
{
    for (int i = 0; i < monkeys.Count; ++i)
    {
        foreach (var (item, index) in monkeys[i].TakeTurn(relief))
        {
            monkeys[index].AddItem(item % modulus);
        }
    }
}

void solve(string file_name, int num_rounds, Relief relief)
{
    List<Monkey> monkeys = parse(file_name);
    long modulus = monkeys.Select(m => m.test_modulus).Product();

    for (int i = 0; i < num_rounds; ++i)
    {
        processRound(monkeys, relief, modulus);
    }

    long monkey_business = monkeys.Select(m => m.inspection_count).OrderByDescending(_ => _).Take(2).Product();
    Console.WriteLine(monkey_business);
}

void part1(string file_name)
{
    Console.WriteLine("part 1 - {0}:", file_name);
    solve(file_name, 20, v => v / 3);
}

void part2(string file_name)
{
    Console.WriteLine("part 2 - {0}:", file_name);
    solve(file_name, 10000, v => v);
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");

class Monkey
{
    List<long> items;
    string operation;
    public int test_modulus;
    int true_index;
    int false_index;
    public long inspection_count;

    public Monkey(List<string> data)
    {
        items = Regex.Matches(data[1], @"\d+").Select(match => long.Parse(match.Value)).ToList();
        operation = Regex.Match(data[2], @"^  Operation: new = (?<operation>.*)$").Groups["operation"].Value;
        test_modulus = int.Parse(Regex.Match(data[3], @"\d+").Value);
        true_index = int.Parse(Regex.Match(data[4], @"\d+").Value);
        false_index = int.Parse(Regex.Match(data[5], @"\d+").Value);
        inspection_count = 0;
    }

    public void AddItem(long item) => items.Add(item);

    private long applyOperation(long value)
    {
        var parseOperand = (string operand) => operand == "old" ? value : long.Parse(operand);
        string[] tokens = operation.Split(' ');
        long operand_1 = parseOperand(tokens.First());
        long operand_2 = parseOperand(tokens.Last());
        long new_value = tokens[1] switch
        {
            "+" => operand_1 + operand_2,
            "*" => operand_1 * operand_2,
        };

        return new_value;
    }

    private bool applyTest(long value) => ((value % test_modulus) == 0);

    public List<(long, int)> TakeTurn(Relief relief)
    {
        List<(long, int)> throw_result = new List<(long, int)>();

        foreach (long item in items)
        {
            ++inspection_count;
            long new_item = applyOperation(item);
            new_item = relief(new_item);
            bool test_result = applyTest(new_item);
            throw_result.Add(new(new_item, (test_result ? true_index : false_index)));
        }

        items.Clear();

        return throw_result;
    }
}

delegate long Relief(long value);