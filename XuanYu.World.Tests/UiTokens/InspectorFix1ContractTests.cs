using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class InspectorFix1ContractTests
{
    static readonly string Panel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));

    [Fact]
    public void Readonly_values_use_inline_copy_presenter_without_selectable_text_template()
    {
        Assert.Contains("InspectorReadOnlyValuePresenter", Panel);
        Assert.DoesNotContain("<xy:XYSelectableText", Panel);
    }

    [Fact]
    public void Inspector_content_host_does_not_mount_legacy_navigation_shells()
    {
        foreach (var legacy in new[] { "MapEditorPanel", "MarkerInspectorPanel", "FeatureInspectorPanel", "EntityInspectorPanel" })
            Assert.DoesNotContain($"<local:{legacy}", Panel);
    }

    [Fact]
    public void Navigation_uses_semantic_rail_item_and_keeps_recent_entry_stable()
    {
        Assert.Contains("XYNavigationRail", Panel);
        Assert.DoesNotContain("<xy:XYButton Content=\"{Binding}\"", Panel);
        Assert.Contains("IsInspectorRecentEmpty", Panel);
    }
}
