using Common;
using System.Collections.Immutable;
using System.Text;
using static Common.Common;

void part1(string file_name)
{
    List<string> input = readInput(file_name);
    List<(PacketList left, PacketList right)> packet_pairs = new List<(PacketList left, PacketList right)>();
    for (int i = 0; i < input.Count; i += 3)
    {
        packet_pairs.Add((new PacketList(input[i]), new PacketList(input[i + 1])));
    }

    long sum = packet_pairs
        .Select((p, index) => (p, index))
        .Where(x => x.p.left.CompareTo(x.p.right) < 0)
        .Select(x => x.index + 1)
        .Sum();

    Console.WriteLine(sum);
}

void part2(string file_name)
{
    List<string> input = readInput(file_name);
    List<PacketList> packets = PacketList.dividerPackets.Select(s => new PacketList(s)).ToList();
    foreach (string s in input)
    {
        if (!string.IsNullOrWhiteSpace(s))
        {
            packets.Add(new PacketList(s));
        }
    }

    long product = packets
        .OrderBy(p => p)
        .Select((p, i) => (p, i))
        .Where(x => (x.p.isDividerPacket()))
        .Select(x => x.i + 1)
        .Product();

    Console.WriteLine(product);
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");

public enum Type
{
    List,
    Integer,
}

public interface Item : IComparable<Item>
{
    Type getType();
}

public class PacketInteger : Item, IComparable<PacketInteger>
{
    public int Value { get; set; }

    public Type getType() => Type.Integer;

    public PacketInteger(StringReader reader)
    {
        while (Char.IsDigit((char)reader.Peek()))
        {
            Value *= 10;
            Value += reader.Read() - '0';
        }
    }

    public override string ToString() => Value.ToString();

    public int CompareTo(Item? other) => (other.getType() == Type.Integer) ? CompareTo((PacketInteger)other) : new PacketList(this).CompareTo((PacketList)other);

    public int CompareTo(PacketInteger? other) => Value.CompareTo(other.Value);
}

public class PacketList : Item, IComparable<PacketList>
{
    public static ImmutableList<string> dividerPackets = ImmutableList.Create("[[2]]", "[[6]]");

    public List<Item> Items { get; set; }

    public PacketList(string s) : this(new StringReader(s))
    {
    }

    private PacketList(StringReader reader)
    {
        Items = new List<Item>();
        char c = (char)reader.Read();  // throw away opening '['

        for (c = (char)reader.Peek(); c != -1; c = (char)reader.Peek())
        {
            if (c == ']')
            {
                reader.Read();
                break;
            }
            else if (c == '[')
            {
                Items.Add(new PacketList(reader));
            }
            else if (c == ',')
            {
                reader.Read();
            }
            else
            {
                Items.Add(new PacketInteger(reader));
            }
        }
    }

    public PacketList(PacketInteger pi) => Items = new List<Item>() { pi };

    public bool isDividerPacket() => dividerPackets.Contains(ToString());

    public Type getType() => Type.List;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append('[');
        if (Items.Count > 0)
        {
            sb.Append(Items.First());
        }
        foreach (Item item in Items.Skip(1))
        {
            sb.Append(',');
            sb.Append(item);
        }
        sb.Append(']');

        return sb.ToString();
    }

    public int CompareTo(PacketList? other)
    {
        for (int i = 0; i < Math.Min(Items.Count, other.Items.Count); ++i)
        {
            int comp = Items[i].CompareTo(other.Items[i]);
            if (comp != 0) return comp;
        }
        return Items.Count.CompareTo(other.Items.Count);
    }

    public int CompareTo(Item? other) => (other.getType() == Type.Integer) ? CompareTo(new PacketList((PacketInteger)other)) : CompareTo((PacketList)other);
}