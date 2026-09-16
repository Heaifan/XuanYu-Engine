using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class InspectorFix1ContractTests
{
    static readonly string Panel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));

    static readonly string ReadOnlyPresenter = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorReadOnlyValuePresenter.axaml"));

    [Fact]
    public void Readonly_values_use_inline_copy_presenter_without_selectable_text_template()
    {
        Assert.Contains("InspectorReadOnlyValuePresenter", Panel);
        Assert.DoesNotContain("<xy:XYSelectableText", Panel);
    }

    [Fact]
    public void Readonly_value_presenter_is_quiet_and_keeps_copy_action()
    {
        Assert.Contains("<xy:XYText", ReadOnlyPresenter);
        Assert.DoesNotContain("<xy:XYTextField", ReadOnlyPresenter);
        Assert.DoesNotContain("<xy:XYSelectableText", ReadOnlyPresenter);
        Assert.DoesNotContain("ToolTip.Tip", ReadOnlyPresenter);
        Assert.Contains("Content=\"复制\"", ReadOnlyPresenter);
        Assert.Contains("AutomationProperties.Name=\"复制属性值\"", ReadOnlyPresenter);
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
