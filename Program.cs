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
                    PrintError($"Invalid operation: --{args[0]}--");
                    return 2;
            }
            return 0;
        }

        static void PrintInfo()
        {
            Console.WriteLine("INFO!");
        }

        public static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }
    }
}
