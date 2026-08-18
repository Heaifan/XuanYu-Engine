using System.Text.Json;

namespace XYUI.Avalonia.Catalog;
public static class XyuiCatalogSource
{
    public static IReadOnlyList<XyuiCatalogEntry> Load()
    {
        var root = XyuiCatalogPaths.FindRepositoryRoot();
        return root is null ? Array.Empty<XyuiCatalogEntry>() : Load(root);
    }

    public static IReadOnlyList<XyuiCatalogEntry> Load(string repositoryRoot)
    {
        var entries = new List<XyuiCatalogEntry>();
        AddFoundation(entries, repositoryRoot);
        for (var phase = 1; phase <= 9; phase++)
        {
            AddPhase(entries, repositoryRoot, phase);
        }
        return entries;
    }

    static void AddFoundation(List<XyuiCatalogEntry> entries, string root)
    {
        var path = Path.Combine(root, "xyui", "registry", "foundation", "identity-map.json");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        foreach (var item in document.RootElement.GetProperty("items").EnumerateArray())
        {
            var source = String(item, "source_item_id");
            var canonical = String(item, "canonical_id");
            entries.Add(Create("XYUI-0", source, canonical, String(item, "display_name"),
                "xyui/registry/foundation/identity-map.json", true));
        }
    }

    static void AddPhase(List<XyuiCatalogEntry> entries, string root, int phase)
    {
        var module = $"XYUI-{phase}";
        var directory = Path.Combine(root, "xyui", "specs", $"XYUI{phase}");
        var mapping = Path.Combine(directory, $"{module}.mapping.json");
        var specification = Path.Combine(directory, $"{module}.canonical.md");
        if (!File.Exists(mapping))
        {
            entries.Add(new(module, module, module, module,
                "SOURCE NOT PRESENT IN CURRENT REPOSITORY", "No preview", "UNRESOLVED", "UNRESOLVED", "UNRESOLVED", "UNRESOLVED", "", "",
                Array.Empty<string>(), new(false, false, false, false, false), false));
            return;
        }
        using var document = JsonDocument.Parse(File.ReadAllText(mapping));
        foreach (var component in document.RootElement.GetProperty("components").EnumerateArray())
        {
            var id = String(component, "component_id");
            var name = String(component, "name");
            var title = String(component, "title");
            var api = component.GetProperty("refs").EnumerateArray()
                .Select(item => String(item, "property")).Distinct().Take(8).ToArray();
            var details = XyuiCatalogSpecReader.Read(specification, name);
            entries.Add(Create(module, id, id, $"{name}｜{title}", $"xyui/specs/XYUI{phase}/{module}.canonical.md",
                File.Exists(specification), api, details));
        }
    }

    static XyuiCatalogEntry Create(string module, string source, string canonical,
        string title, string specification, bool documented, IReadOnlyList<string>? api = null,
        XyuiCatalogDetails? details = null)
    {
        var name = title.Split(new[] { '|', '｜' })[0].Trim().Split('/')[0].Trim();
            var avalonia = XyuiCatalogTypeMap.Types.TryGetValue(source, out var type);
            var present = true;
        var status = new XyuiCatalogStatus(true, true, avalonia, XyuiCatalogTypeMap.GalleryIds.Contains(source), documented);
        return new(module, source, canonical, name, title, details?.Description ?? title,
            details?.Preview ?? "Foundation runtime", details?.Variants ?? "See canonical spec",
            details?.States ?? "See canonical spec", details?.Usage ?? "See canonical spec", specification,
            type ?? "", api ?? Array.Empty<string>(), status, present);
    }

    static string String(JsonElement element, string property) =>
        element.TryGetProperty(property, out var value) ? value.GetString() ?? "" : "";
}
