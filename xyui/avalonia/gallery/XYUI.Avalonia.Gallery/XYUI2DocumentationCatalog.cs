using System.Text.Json;
using Avalonia.Controls;
using XYUI.Avalonia.Catalog;

namespace XYUI.Avalonia.Gallery;

public static partial class XYUI2DocumentationCatalog
{
    public static readonly IReadOnlySet<string> BatchIds = new HashSet<string>
    {
        "XYUI-2-01", "XYUI-2-02", "XYUI-2-03", "XYUI-2-04", "XYUI-2-05", "XYUI-2-06",
        "XYUI-2-07", "XYUI-2-08", "XYUI-2-09", "XYUI-2-10", "XYUI-2-11", "XYUI-2-12",
        "XYUI-2-13", "XYUI-2-14", "XYUI-2-15", "XYUI-2-16", "XYUI-2-17", "XYUI-2-18",
        "XYUI-2-19", "XYUI-2-20", "XYUI-2-21", "XYUI-2-22", "XYUI-2-23", "XYUI-2-24"
    };

    public static IReadOnlyList<XYUI1ComponentDocument> Build() =>
        XyuiCatalogSource.Load().Where(x => x.Module == "XYUI-2" && BatchIds.Contains(x.SourceItemId))
            .Select(Create).ToArray();

    static XYUI1ComponentDocument Create(XyuiCatalogEntry entry)
    {
        var id = entry.SourceItemId;
        var type = entry.AvaloniaType.Split('.').Last();
        var name = ChineseName(entry);
        var variants = Phase2AVariants(id) is { Count: > 0 } p2v ? p2v : Phase2BVariants(id) is { Count: > 0 } p2bv ? p2bv : Names(entry.Variants).Select(x => new XYUIDocVariant(x, "", "")).ToArray();
        var states = Phase2AStates(id) is { Count: > 0 } p2s ? p2s : Phase2BStates(id) is { Count: > 0 } p2bs ? p2bs : Names(entry.States).Select(x => new XYUIDocState(x, "")).ToArray();
        var props = Phase2AProperties(id) is { Count: > 0 } p2p ? p2p : Phase2BProperties(id) is { Count: > 0 } p2bp ? p2bp : Properties(id);
        return new(id, name, type, entry.Description,
            entry.Usage == "See canonical spec" ? entry.States : entry.Usage,
            () => XYUI2GalleryCatalog.CreatePreview(id),
            Usages(id, type), variants, states, props, Tokens(id), type)
        {
            CanonicalIdentity = entry.CanonicalIdentity, KnownGap = entry.KnownGap,
            Acceptance = PendingAcceptance,
            QuickStartXaml = Phase2AQuickStart(id) is { Length: > 0 } q2a ? q2a : Phase2BQuickStart(id),
            CoreRules = Phase2ACoreRules(id) is { Count: > 0 } r2a ? r2a : Phase2BCoreRules(id),
            FoundationMappings = Phase2AFoundationMappings(id) is { Count: > 0 } f2a ? f2a : Phase2BFoundationMappings(id),
            HowToUse = Phase2AHowToUse(id) is { Count: > 0 } h2a ? h2a : Phase2BHowToUse(id),
            LiveExamplesFactory = XYUI2LiveExamplesFactory.Supports(id)
                ? () => XYUI2LiveExamplesFactory.Create(id)!
                : null
        };
    }

    public const string PendingAcceptance = "READY FOR USER VISUAL ACCEPTANCE";

    static string[] Names(string block) => block == "None defined" || block == "See canonical spec"
        ? []
        : block.Split('；', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    static string ChineseName(XyuiCatalogEntry entry) => entry.Title.Split('｜').Last().Split('/').Last().Trim();

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
