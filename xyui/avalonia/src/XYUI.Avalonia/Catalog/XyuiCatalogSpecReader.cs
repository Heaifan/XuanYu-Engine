namespace XYUI.Avalonia.Catalog;

internal static class XyuiCatalogSpecReader
{
    public static XyuiCatalogDetails? Read(string path, string name)
    {
        if (!File.Exists(path)) return null;
        var lines = File.ReadAllLines(path);
        var start = Array.FindIndex(lines, line => line.StartsWith("- ") && line.Contains("·") && line.Contains(name));
        if (start < 0) return null;
        var end = Array.FindIndex(lines, start + 1, line => line.StartsWith("- "));
        if (end < 0) end = lines.Length;
        return new(Block(lines, start, end, "用途"),
            "Canonical preview → Gallery example when AVALONIA is present",
            Block(lines, start, end, "变体"), Block(lines, start, end, "状态"),
            Block(lines, start, end, "使用场景"));
    }

    static string Block(string[] lines, int start, int end, string heading)
    {
        var marker = Array.FindIndex(lines, start, end - start, line => line.Trim() == $"- {heading}");
        if (marker < 0) return "UNRESOLVED in canonical spec";
        var values = new List<string>();
        for (var i = marker + 1; i < end && !lines[i].StartsWith("    - "); i++)
        {
            var value = lines[i].Trim();
            if (value.StartsWith("- ")) values.Add(value[2..]);
        }
        return values.Count == 0 ? "UNRESOLVED in canonical spec" : string.Join("；", values);
    }
}

public sealed record XyuiCatalogDetails(
    string Description, string Preview, string Variants, string States, string Usage);
