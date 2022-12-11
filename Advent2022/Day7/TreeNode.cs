namespace Day7
{
    internal class TreeNode
    {
        public string name;
        public TreeNode? parent;
        public List<TreeNode> children;

        long size;

        public TreeNode(string name, TreeNode? parent = null, long size = 0)
        {
            this.name = name;
            this.parent = parent;
            children = new List<TreeNode>();
            this.size = size;
        }

        public TreeNode addChild(TreeNode node)
        {
            if(children.Where(n => n.name == node.name).Count() == 0)
            {
                children.Add(node);
            }
            return this;
        }

        public long TotalSize() => size + children.Sum(c => c.TotalSize());

        public bool isDir() => size == 0;
    }
}
