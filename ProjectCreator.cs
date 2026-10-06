using System.IO.Compression;


namespace Program
{
    public class ProjectCreator
    {
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
            string projectPath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + projectName;
            if (templateName == null)
            {
                UnzipTemplate("empty", projectPath);
            }
            else
            {
                UnzipTemplate(templateName, projectPath);
            }
            return true;
        }

        static bool UnzipTemplate(string templateName, string extractPath)
        {
            string templatePath = AppContext.BaseDirectory + Path.DirectorySeparatorChar + templateName;
            try
            {
                ZipFile.ExtractToDirectory(templatePath, extractPath);
            }
            catch (FileNotFoundException)
            {
                Program.PrintError($"Template {templateName} not found!");
                return false;
            }
            catch (InvalidDataException)
            {
                Program.PrintError("Template file is corrupt or malformed!");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                Program.PrintError("Missing read or write permissions!");
                return false;
            }
            catch (Exception e)
            {
                Program.PrintError("Unknown error occured!");
                Program.PrintError($"Message: {e.Message}");
                return false;
            }
            return true;
        }
    }
}
