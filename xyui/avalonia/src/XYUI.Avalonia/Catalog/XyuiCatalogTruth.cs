using System.Text.Json;

namespace XYUI.Avalonia.Catalog;

internal static class XyuiCatalogTruth
{
    public static IReadOnlyDictionary<string, string> ReadIdentity(string directory, string module)
    {
        var path = Path.Combine(directory, $"{module}.identity.json");
        if (!File.Exists(path)) return new Dictionary<string, string>();
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        return document.RootElement.GetProperty("components").EnumerateArray().ToDictionary(
            x => x.GetProperty("id").GetString() ?? "", x => x.GetProperty("canonical").GetString() ?? "");
    }

    public static IReadOnlyDictionary<string, string> ReadGaps(string directory, string module)
    {
        var path = Path.Combine(directory, $"{module}.gaps.json");
        if (!File.Exists(path)) return new Dictionary<string, string>();
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var result = new Dictionary<string, string>();
        foreach (var gap in document.RootElement.GetProperty("gaps").EnumerateArray())
        {
            var id = gap.GetProperty("gap_id").GetString() ?? "";
            if (!gap.TryGetProperty("component_ids", out var components)) continue;
            foreach (var component in components.EnumerateArray()) result[component.GetString() ?? ""] = id;
        }
        return result;
    }
}
