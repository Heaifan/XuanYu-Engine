using System.IO;

public static class TestSourceRoot
{
    public static string Root { get; } = Resolve();

    public static string ProjectOutput(string project, string file)
    {
        foreach (var configuration in new[] { "Debug", "Release" })
        {
            var path = Path.Combine(Root, project, "bin", configuration, "net10.0", file);
            if (File.Exists(path)) return path;
        }
        return Path.Combine(Root, project, "bin", "Debug", "net10.0", file);
    }

    static string Resolve()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "XuanYu.Engine.slnx")))
                return directory.FullName;

        throw new DirectoryNotFoundException("Could not locate the XuanYu source root.");
    }
}
