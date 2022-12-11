using static Common.Common;
using Day7;

TreeNode parse(string file_name)
{
    List<string> input = readInput(file_name);
    TreeNode root = new TreeNode("/");
    TreeNode? curr = null;

    foreach (string line in input)
    {
        curr = line switch
        {
            "$ cd /" => root,
            "$ cd .." => curr.parent,
            _ when line.StartsWith("$ cd") => curr.children.First(c => c.name == line.Split(' ').Last()),
            _ when line.StartsWith("dir ") => curr.addChild(new TreeNode(line.Split(' ').Last(), curr)),
            _ when line != "$ ls" => curr.addChild(new TreeNode(line.Split(' ').Last(), curr, long.Parse(line.Split(' ').First()))),
            _ => curr,
        };
    }

    return root;
}

List<long> getDirSizes(TreeNode node)
{
    List<long> dir_sizes = new List<long>();

    foreach (TreeNode child in node.children)
    {
        dir_sizes.AddRange(getDirSizes(child));
    }

    if (node.isDir())
    {
        dir_sizes.Add(node.TotalSize());
    }

    return dir_sizes;
}

void part1(string file_name)
{
    TreeNode root = parse(file_name);
    Console.WriteLine(getDirSizes(root).Where(size => size <= 100000).Sum());
}

void part2(string file_name)
{
    TreeNode root = parse(file_name);
    long need_to_free = root.TotalSize() - 70000000 + 30000000;
    Console.WriteLine(getDirSizes(root).Where(size => size >= need_to_free).Min());
}

part1("sample_input.txt");
part1("input.txt");
part2("sample_input.txt");
part2("input.txt");