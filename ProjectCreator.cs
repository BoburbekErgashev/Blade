using System.IO.Compression;


namespace Program
{
    public class ProjectCreator
    {
        static string templateFolderPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "templates"));

        public static bool CreateProject(string[] args)
        {
            string? projectName = null;
            string? templateName = null;
            for (uint i = 1; i < args.Length; i++)
            {
                if (i == 1)
                {
                    projectName = args[i];
                }
                else if (i == 2)
                {
                    templateName = args[i];
                }
                else
                {
                    break;
                }
            }
            if (projectName == null)
            {
                Program.PrintError("Project name cannot be  empty!");
                return false;
            }
            if (projectName.Contains("/"))
            {
                if (!Program.Confirmation("Continue", "Project name contains slash('/') and will be interpreted as path"))
                {
                    return false;
                }
            }
            UnzipTemplate(templateName, projectName);
            return true;
        }

        static bool UnzipTemplate(string? templateName, string projectName)
        {
            string projectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, projectName));
            if (Directory.Exists(projectPath))
            {
                Program.PrintError("Project cannot be created:", $"Directory {projectPath} already exists!");
                return false;
            }
            string templatePath;
            if (templateName == null)
            {
                string? choosenTemplate = ChooseTemplate();
                if (choosenTemplate == null)
                {
                    return false;
                }
                templatePath = choosenTemplate;
            }
            else
            {
                templatePath = Path.Combine(templateFolderPath, templateName + ".zip");
            }
            try
            {
                ZipFile.ExtractToDirectory(templatePath, projectPath);
            }
            catch (FileNotFoundException)
            {
                Program.PrintError($"Template '{templateName}' not found!");
                return false;
            }
            catch (Exception e)
            {
                Program.PrintError("An error occured!", $"Message: {e.Message}");
                return false;
            }
            return true;
        }
        static string? ChooseTemplate()
        {
            string[] templatesList = Directory.GetFiles(templateFolderPath, "*.zip", SearchOption.AllDirectories);
            if (templatesList.Length == 0)
            {
                Program.PrintError("No usable templates found!");
                return null;
            }
            Console.WriteLine("Choose a template\n");
            for (uint i = 0; i < templatesList.Length; i++)
            {
                Console.WriteLine($"[{i}] {Path.GetFileNameWithoutExtension(templatesList[i])}");
            }
            Console.Write("Write a template index | default [0]: ");
            byte repeat = 3;
            for (byte i = 0; i < repeat; i++)
            {
                if (i != 0)
                {
                    Console.Write("Please enter a valid index: ");
                }
                string? answer = Console.ReadLine();
                if (answer == null)
                {
                    Console.Write("\n");
                }
                if (answer == string.Empty)
                {
                    return templatesList[0];
                }
                if (int.TryParse(answer, out int templateIndex))
                {
                    if (templateIndex >= 0 && templateIndex < templatesList.Length)
                    {
                        return templatesList[templateIndex];
                    }
                }
            }
            Console.WriteLine("Failed to select a template. Using default option");
            return templatesList[0];
        }
    }
}
