using System.IO.Compression;


namespace Program
{
    public class ProjectCreator
    {
        static string templateFolderPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "templates"));

        public static bool CreateProject(string[] args)
        {
            return true;
        }
    }
}
