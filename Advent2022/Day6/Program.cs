using static Common.Common;

int findStartOfPacket(string buffer, int packet_length)
{
    int count = buffer.Length - packet_length;
    return Enumerable.Range(packet_length, count).First(i => buffer.AsSpan(i - packet_length, packet_length).ToArray().Distinct().Count() == packet_length);
}

void findMarker(string file_name, int marker_length)
{
    Console.WriteLine(file_name);
    foreach (string line in readInput(file_name))
    {
        int start = findStartOfPacket(line, marker_length);
        //Console.WriteLine(line);
        //Console.Write(new string(' ', start - marker_length) + '*' + new string('-', marker_length - 2) + "* = ");
        Console.WriteLine(start);
    }
    Console.WriteLine();
}

void part1(string file_name)
{
    findMarker(file_name, 4);
}

void part2(string file_name)
{
    findMarker(file_name, 14);
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");