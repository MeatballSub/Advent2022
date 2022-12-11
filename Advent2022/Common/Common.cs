namespace Common
{
    public static class Common
    {
        public static List<string> readInput(string file_name)
        {
            List<string> input = new List<string>();
            using (TextReader reader = File.OpenText(file_name))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    input.Add(line);
                }
            }
            return input;
        }
    }

    public static class ExtensionHelper
    {
        public static long Product(this IEnumerable<long> source)
        {
            return source.Aggregate((long)1, (a, b) => a * b);
        }

        public static long Product(this IEnumerable<int> source)
        {
            return source.Aggregate((long)1, (a, b) => a * b);
        }

    }
}