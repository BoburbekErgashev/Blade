namespace Program
{
    internal class Program
    {
        static int Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintInfo();
                return 0;
            }
            switch (args[0].ToLower())
            {
                case "new":
                    ProjectCreator.CreateProject(args);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid operation: --{args[0]}--");
                    Console.ResetColor();
                    return 2;
            }
            return 0;
        }

        static void PrintInfo()
        {
            Console.WriteLine("INFO!");
        }
    }
}
