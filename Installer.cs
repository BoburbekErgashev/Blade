namespace Program
{
    public class Installer
    {
        public static bool Install()
        {
            if (!OperatingSystem.IsLinux())
            {
                Program.PrintError("This operation is only supported on Linux operating system");
                return false;
            }
            string? binPath = getExecFullPath();
            if (binPath == null)
            {
                return false;
            }

        }

        private static string? getExecFullPath()
        {
            string? calledPath = Environment.ProcessPath;
            if (string.IsNullOrEmpty(calledPath))
            {
                Program.PrintError("Failed to resolve blade executable path!");
                return null;
            }
            FileInfo fi = new FileInfo(calledPath);
            FileSystemInfo? fsi = fi.ResolveLinkTarget(true);
            if (fsi == null)
            {
                return calledPath;
            }
            return fsi.FullName;
        }

        private static string? getLocalBinPath()
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            if (string.IsNullOrEmpty(homeDir))
            {
                Program.PrintError("Failed to fetch local bin directory!");
                return null;
            }
            string binDir = Path.Combine(homeDir, ".local", "bin");
            try
            {
                Directory.CreateDirectory(binDir);
                return binDir;
            }
            catch
            {
                Program.PrintError("Failed to create symbolic link!");
                return null;
            }
        }

    }
}
