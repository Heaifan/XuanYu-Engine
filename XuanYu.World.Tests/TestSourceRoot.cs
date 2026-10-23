using System.IO;

public static class TestSourceRoot
{
    public static string Root { get; } = Resolve();

    public static string ProjectOutput(string project, string file)
    {
        var artifacts = Environment.GetEnvironmentVariable("XYE_BUILD_DIRECTORY");
        foreach (var configuration in new[] { "debug", "release" })
        {
            var path = Path.Combine(artifacts ?? string.Empty, "bin", project,
                configuration, file);
            if (!string.IsNullOrWhiteSpace(artifacts) && File.Exists(path)) return path;
        }
        return Path.Combine(Root, project, "bin", "Debug", "net10.0", file);
    }

    static string Resolve()
    {
        var configured = Environment.GetEnvironmentVariable("XYE_WORKSPACE_SOURCE_DIRECTORY");
        if (!string.IsNullOrWhiteSpace(configured) &&
            File.Exists(Path.Combine(configured, "XuanYu.Engine.slnx")))
            return Path.GetFullPath(configured);

        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "XuanYu.Engine.slnx")))
                return directory.FullName;

        throw new DirectoryNotFoundException("Could not locate the XuanYu source root.");
    }
}
