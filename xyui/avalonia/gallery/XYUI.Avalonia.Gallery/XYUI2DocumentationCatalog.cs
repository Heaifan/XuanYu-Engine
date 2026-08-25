using System.Text.Json;
using Avalonia.Controls;
using XYUI.Avalonia.Catalog;

namespace XYUI.Avalonia.Gallery;

// XYUI-2 文档目录：Batch 01（Button / IconButton / ToggleButton）。
// 描述/变体/状态取自 canonical spec reader；Token 表直接读 mapping.json（单一事实源）。
public static class XYUI2DocumentationCatalog
{
    public static readonly IReadOnlySet<string> BatchIds = new HashSet<string>
    {
        "XYUI-2-01", "XYUI-2-02", "XYUI-2-03"
    };

    public static IReadOnlyList<XYUI1ComponentDocument> Build() =>
        XyuiCatalogSource.Load().Where(x => x.Module == "XYUI-2" && BatchIds.Contains(x.SourceItemId))
            .Select(Create).ToArray();

    static XYUI1ComponentDocument Create(XyuiCatalogEntry entry)
    {
        var type = entry.AvaloniaType.Split('.').Last();
        return new(entry.SourceItemId, entry.Name, type, entry.Description,
            entry.Usage == "See canonical spec" ? entry.States : entry.Usage,
            () => XYUI2GalleryCatalog.CreatePreview(entry.SourceItemId),
            Usages(entry.SourceItemId, type), Names(entry.Variants).Select(x => new XYUIDocVariant(x, "", "")).ToArray(),
            Names(entry.States).Select(x => new XYUIDocState(x, "")).ToArray(),
            Array.Empty<XYUIDocProperty>(), Tokens(entry.SourceItemId), type)
        { CanonicalIdentity = entry.CanonicalIdentity, KnownGap = entry.KnownGap };
    }

    static string[] Names(string block) => block == "None defined" || block == "See canonical spec"
        ? []
        : block.Split('；', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    static string[] Usages(string id, string type) => id switch
    {
        "XYUI-2-01" => [$"<c:{type} Content=\"新建\" />", "<c:XYButton Content=\"删除\" Variant=\"Danger\" />"],
        "XYUI-2-02" => [$"<c:{type} Icon=\"Search\" AutomationProperties.Name=\"搜索\" IsSelected=\"true\" />"],
        "XYUI-2-03" => [$"<c:{type} Content=\"网格吸附\" IsChecked=\"true\" />"],
        _ => [$"<c:{type} />"]
    };

    static IReadOnlyList<XYUIDocToken> Tokens(string id)
    {
        var root = FindRepositoryRoot();
        if (root is null) return [];
        var path = Path.Combine(root, "xyui", "specs", "XYUI2", "XYUI-2.mapping.json");
        using var document = JsonDocument.Parse(File.ReadAllText(path));
        var component = document.RootElement.GetProperty("components").EnumerateArray()
            .Single(x => x.GetProperty("component_id").GetString() == id);
        return component.GetProperty("refs").EnumerateArray().Select(item => new XYUIDocToken(
            item.GetProperty("property").GetString() ?? "",
            item.GetProperty("value").GetString() ?? "",
            item.GetProperty("kind").GetString() ?? "")).ToArray();
    }

    static string? FindRepositoryRoot()
    {
        foreach (var start in new[] { Environment.CurrentDirectory, AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(start);
            while (directory is not null)
            {
                if (File.Exists(Path.Combine(directory.FullName, "xyui", "registry", "foundation", "identity-map.json")))
                    return directory.FullName;
                directory = directory.Parent;
            }
        }
        return null;
    }
}
