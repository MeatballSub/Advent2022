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
}