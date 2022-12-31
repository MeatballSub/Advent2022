using static Common.Common;

void part1(string file_name)
{
    MyDataStructure problem = new MyDataStructure(readInput(file_name), 1);
    problem.Mix();
    Console.WriteLine(problem.coordinateSum());
}

void part2(string file_name)
{
    MyDataStructure problem = new MyDataStructure(readInput(file_name), 2);
    for (int i = 0; i < 10; ++i)
    {
        problem.Mix();
    }
    Console.WriteLine(problem.coordinateSum());
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");

class MyDataStructure
{
    public class Node
    {
        public long value { get; set; }
        public Node? prev { get; set; }
        public Node? next { get; set; }

        public Node(long value, Node? prev, Node? next)
        {
            this.value = value;
            this.prev = prev;
            this.next = next;
        }
    }

    public List<Node> original_list { get; init; } = new();
    private int zero_original_index { get; set; }
    public Node head = null;
    public Node tail = null;

    private bool isEmpty()
    {
        return head == null && tail == null;
    }

    private void Add(long value)
    {
        if (value == 0)
        {
            zero_original_index = original_list.Count;
        }

        Node node;
        if (isEmpty())
        {
            node = new Node(value, null, null);
            head = node;
            tail = node;
            node.prev = node;
            node.next = node;
        }
        else
        {
            node = new Node(value, tail, head);
            head.prev = node;
            tail.next = node;
            tail = node;
        }
        original_list.Add(node);
    }

    public MyDataStructure(List<string> input, int part)
    {
        (part switch
        {
            1 => input.Select(_ => long.Parse(_)).ToList(),
            2 => input.Select(_ => long.Parse(_) * 811589153).ToList(),
            _ => throw new ArgumentOutOfRangeException()
        }).ForEach(Add);
    }

    public void Mix()
    {
        for (int i = 0; i < original_list.Count; i++)
        {
            Node curr = original_list[i];
            if (curr.value != 0)
            {
                Node new_prev = curr;
                Node new_next = curr;
                if (curr.value > 0)
                {
                    for (int j = 0; j < curr.value % (original_list.Count - 1); ++j)
                    {
                        new_prev = new_prev.next;
                    }
                    new_next = new_prev.next;
                }
                else if (curr.value < 0)
                {
                    for (int j = 0; j < (-curr.value) % (original_list.Count - 1); ++j)
                    {
                        new_next = new_next.prev;
                    }
                    new_prev = new_next.prev;
                }

                curr.next.prev = curr.prev;
                curr.prev.next = curr.next;
                new_next.prev = curr;
                new_prev.next = curr;
                curr.prev = new_prev;
                curr.next = new_next;
            }
        }
    }

    public long coordinateSum()
    {
        long sum = 0;
        int movement = 1000 % original_list.Count;
        Node curr = original_list[zero_original_index];
        for (int i = 0; i < 3; ++i)
        {
            for (int j = 0; j < movement; ++j)
            {
                curr = curr.next;
            }
            sum += curr.value;
        }
        return sum;
    }
}