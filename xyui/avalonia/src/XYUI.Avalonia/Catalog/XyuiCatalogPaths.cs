namespace XYUI.Avalonia.Catalog;

internal static class XyuiCatalogPaths
{
    public static string? FindRepositoryRoot()
    {
        foreach (var start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(start);
            while (directory is not null)
            {
                var registry = Path.Combine(directory.FullName, "xyui", "registry", "foundation", "identity-map.json");
                var spec = Path.Combine(directory.FullName, "xyui", "specs", "XYUI1", "XYUI-1.mapping.json");
                if (File.Exists(registry) && File.Exists(spec))
                {
                    return directory.FullName;
                }
                directory = directory.Parent;
            }
        }
        return null;
    }
}
