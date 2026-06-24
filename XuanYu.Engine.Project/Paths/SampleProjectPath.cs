using FluidWarfare.Core.Results;

namespace FluidWarfare.Project.Paths;

public static class SampleProjectPath
{
    public static EngineResult TryFindFrom(string startDirectory, out string projectDirectory)
    {
        projectDirectory = string.Empty;

        if (string.IsNullOrWhiteSpace(startDirectory))
        {
            return EngineResult.Fail(EngineError.Create(
                "Project.StartDirectoryMissing",
                "起始目录不能为空。"));
        }

        var current = new DirectoryInfo(startDirectory);
        while (current is not null)
        {
            var candidate = Path.Combine(current.FullName, "GameProjects", "SampleProject");
            var manifestPath = Path.Combine(candidate, "game.project.json");

            if (File.Exists(manifestPath))
            {
                projectDirectory = candidate;
                return EngineResult.Success();
            }

            current = current.Parent;
        }

        return EngineResult.Fail(EngineError.Create(
            "Project.SampleProjectMissing",
            "未找到示例项目 GameProjects/SampleProject。"));
    }
}
