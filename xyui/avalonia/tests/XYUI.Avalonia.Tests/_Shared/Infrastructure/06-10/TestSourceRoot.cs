namespace XYUI.Avalonia.Tests;

internal static class TestSourceRoot
{
    public static string Root { get; } = Resolve();

    static string Resolve()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory);
             directory is not null; directory = directory.Parent)
            if (File.Exists(Path.Combine(directory.FullName, "XuanYu.Engine.slnx")))
                return directory.FullName;

        throw new DirectoryNotFoundException("Could not locate the XuanYu source root.");
    }
}
