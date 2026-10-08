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

        public static void PrintError(params ReadOnlySpan<string> messages)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (string m in messages)
            {
                Console.Error.WriteLine(m);
            }
            Console.ResetColor();
        }

        public static bool Confirmation(string question, params ReadOnlySpan<string> messages)
        {
            foreach (string m in messages)
            {
                Console.WriteLine(m);
            }
            Console.Write($"\n{question}? [y/N] : ");
            byte repeat = 3;
            for (byte i = 0; i < repeat; i++)
            {
                if (i != 0)
                {
                    Console.Write("Please answer with 'y' or 'n' | default [N]: ");
                }
                string? answer = Console.ReadLine();
                if (answer == null)
                {
                    Console.Write("\n");
                    return false;
                }
                if (answer == string.Empty)
                {
                    return false;
                }
                if (answer.Trim().ToLower() is "y" or "yes")
                {
                    return true;
                }
            }
            return true;
        }
    }
}
