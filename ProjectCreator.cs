namespace Program
{
    public class ProjectCreator
    {
        private static readonly string defCmake = """
        cmake_minimum_required(VERSION 3.16)

        include(CmakeConfig/project.cmake)
        include(CmakeConfig/sdklibdirs.cmake)
        include(CmakeConfig/sourcefiles.cmake)

        project(${BLADE_PROJECT_NAME} CXX C)

        set(CMAKE_CXX_STANDARD 20)
        set(CMAKE_CXX_STANDARD_REQUIRED ON)

        set(SDL_SHARED OFF CACHE BOOL "" FORCE)
        set(SDL_STATIC ON  CACHE BOOL "" FORCE)
        set(SDL_TESTS  OFF CACHE BOOL "" FORCE)

        add_subdirectory(${SDL_LIB_DIR} "${CMAKE_CURRENT_SOURCE_DIR}/build/sdl" EXCLUDE_FROM_ALL)
        add_subdirectory(${ENGINE_LIB_DIR} "${CMAKE_CURRENT_SOURCE_DIR}/build/engine")


        add_executable(${BLADE_PROJECT_NAME} ${SOURCE_FILES})

        target_include_directories(${BLADE_PROJECT_NAME} PRIVATE include)

        target_link_libraries(${BLADE_PROJECT_NAME} PRIVATE blade-graphics SDL3::SDL3-static)
        """;

        public static bool CreateProject(string[] args)
        {
            if (args.Length < 2)
            {
                Program.PrintError("Project name is required!");
                return false;
            }
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                if (args[1].Contains(c))
                {
                    Program.PrintError($"Project name cannot contain '{c}' character!");
                    return false;
                }
            }
            if (!CreateEmptyProject(args[1], Environment.CurrentDirectory))
            {
                Program.PrintError("Failed to create the project!");
                return false;
            }
            return true;
        }

        static bool CreateEmptyProject(string projectName, string projectPath)
        {
            string fullProjectDir = Path.Combine(projectPath, projectName);
            try
            {
                Directory.CreateDirectory(fullProjectDir);
                Directory.CreateDirectory(Path.Combine(fullProjectDir, "src"));
                Directory.CreateDirectory(Path.Combine(fullProjectDir, "include"));
                Directory.CreateDirectory(Path.Combine(fullProjectDir, "CmakeConfig"));
                Directory.CreateDirectory(Path.Combine(fullProjectDir, "build", "debug"));
                Directory.CreateDirectory(Path.Combine(fullProjectDir, "build", "release"));
                Directory.CreateDirectory(Path.Combine(fullProjectDir, "build", "engine"));
                Directory.CreateDirectory(Path.Combine(fullProjectDir, "build", "sdl"));
                File.Create(Path.Combine(fullProjectDir, "CMakeLists.txt")).Close();
                File.Create(Path.Combine(Path.Combine(fullProjectDir, "CmakeConfig"), "project.cmake")).Close();
                File.Create(Path.Combine(Path.Combine(fullProjectDir, "CmakeConfig"), "sdklibdirs.cmake")).Close();
                File.Create(Path.Combine(Path.Combine(fullProjectDir, "CmakeConfig"), "sourcefiles.cmake")).Close();
                File.Create(Path.Combine(Path.Combine(fullProjectDir, "CmakeConfig"), "toolchain.cmake")).Close();
                File.Create(Path.Combine(Path.Combine(fullProjectDir, "CmakeConfig"), "zig.cmake")).Close();
                if (!ConfigureProject(projectName, fullProjectDir))
                {
                    try
                    {
                        Directory.Delete(fullProjectDir, true);
                    }
                    finally { }
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                Program.PrintError("An Error occured!", e.Message);
                return false;
            }
        }

        public static bool ConfigureProject(string projectName, string projectPath)
        {
            string sdlLibPath, engineLibPath;
            string? compilerPath;
            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "engine", "CMakeLists.txt")) || !File.Exists(Path.Combine(AppContext.BaseDirectory, "external", "sdl3", "CMakeLists.txt")))
            {
                Program.PrintError("Failed to fetch engine and sdl3 library locations");
                if (Program.Confirmation("Enter library paths manually"))
                {
                    (string sdlPath, string enginePath)? customPaths = getCustomLibPaths();
                    if (customPaths == null)
                    {
                        return false;
                    }
                    sdlLibPath = customPaths.Value.sdlPath;
                    engineLibPath = customPaths.Value.enginePath;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                sdlLibPath = Path.Combine(AppContext.BaseDirectory, "external", "sdl3");
                engineLibPath = Path.Combine(AppContext.BaseDirectory, "engine");
            }
            if (!File.Exists(Path.Combine(AppContext.BaseDirectory, "bin", "zig", "zig")))
            {
                Program.PrintError("Failed to fetch zig compiler location");
                if (Program.Confirmation("Enter zig compiler path manually"))
                {
                    compilerPath = getCustomCompilerPath();
                    if (compilerPath == null)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                compilerPath = Path.Combine(AppContext.BaseDirectory, "bin", "zig", "zig");
            }
            try
            {
                File.WriteAllText(Path.Combine(projectPath, "CMakeLists.txt"), defCmake);
                File.WriteAllText(Path.Combine(projectPath, "CmakeConfig", "sdklibdirs.cmake"), $"{Program.CreateCmakeVar("SDL_LIB_DIR", sdlLibPath)}\n{Program.CreateCmakeVar("ENGINE_LIB_DIR", engineLibPath)}");
                File.WriteAllText(Path.Combine(projectPath, "CmakeConfig", "zig.cmake"), Program.CreateCmakeVar("ZIG_BIN", compilerPath));
                File.WriteAllText(Path.Combine(projectPath, "CmakeConfig", "project.cmake"), Program.CreateCmakeVar("BLADE_PROJECT_NAME", projectName));
                return true;
            }
            catch (Exception e)
            {
                Program.PrintError("An Error occured!", e.Message);
                return false;
            }
        }

        private static (string sdlPath, string enginePath)? getCustomLibPaths()
        {
            Console.Write("Enter SDL3 library path: ");
            string? sdlLibPath = Console.ReadLine();
            if (string.IsNullOrEmpty(sdlLibPath))
            {
                Program.PrintError("Invalid input!");
                return null;
            }
            if (!File.Exists(Path.Combine(sdlLibPath, "CMakeLists.txt")))
            {
                Program.PrintError("Directory does not contain root CMakeLists.txt file!");
                return null;
            }
            Console.Write("Enter engine library path: ");
            string? engineLibPath = Console.ReadLine();
            if (string.IsNullOrEmpty(engineLibPath))
            {
                Program.PrintError("Invalid input!");
                return null;
            }
            if (!File.Exists(Path.Combine(engineLibPath, "CMakeLists.txt")))
            {
                Program.PrintError("Directory does not contain root CMakeLists.txt file!");
                return null;
            }
            return (sdlLibPath, engineLibPath);
        }


        private static string? getCustomCompilerPath()
        {
            Console.Write("Enter path to directory containing zig compiler binary or path to compiler binary itself: ");
            string? zigPath = Console.ReadLine();
            if (string.IsNullOrEmpty(zigPath))
            {
                Program.PrintError("Invalid input!");
                return null;
            }
            if (Directory.Exists(zigPath) || File.Exists(zigPath))
            {
                FileAttributes attr = File.GetAttributes(zigPath);
                if (attr.HasFlag(FileAttributes.Directory))
                {
                    if (File.Exists(Path.Combine(zigPath, "zig")))
                    {
                        return Path.Combine(zigPath, "zig");
                    }
                    else
                    {
                        Program.PrintError("Directory does not contain zig compiler binary!");
                        return null;
                    }
                }
                return zigPath;
            }
            Program.PrintError("Invalid input!");
            return null;
        }
    }
}
