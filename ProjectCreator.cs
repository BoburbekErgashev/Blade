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
            CreateEmptyProject("gfff", Environment.CurrentDirectory);
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
                return true;
            }
            catch (Exception e)
            {
                Program.PrintError("An Error occured!", e.Message);
                return false;
            }
        }

        public static void ConfigureProject(string projectPath)
        {
            try
            {
                File.WriteAllText(Path.Combine(projectPath, "CMakeLists.txt"), )
            }
            catch (Exception e)
            {
                Program.PrintError("An Error occured!", e.Message);``
            }
        }
    }
}
