using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class InspectorFix2ContractTests
{
    static readonly string Panel = File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
        "..", "..", "..", "..", "XuanYu.Editor.UI", "Right", "InspectorPanel.axaml"));

    [Fact]
    public void Inspector_uses_fixed_width_xyui_navigation_rail()
    {
        Assert.Contains("xy:XYNavigationRail", Panel);
        Assert.Contains("ColumnDefinitions=\"40,8,*\"", Panel);
        Assert.DoesNotContain("ColumnDefinitions=\"108,*\"", Panel);
    }

    [Fact]
    public void Inspector_rail_does_not_use_action_toggle_button_items()
    {
        Assert.DoesNotContain("inspectorNavigationItem", Panel);
        Assert.DoesNotContain("xy:XYToggleButton Content=\"{Binding Category}\"", Panel);
    }

    [Fact]
    public void Geometry_inspector_projection_is_empty_without_geometry_selection()
    {
        var vm = new XuanYu.Editor.UI.UiVm(null, seedInitialScene: false);

        Assert.Equal("", vm.InspectorFeatureTypeText);
        Assert.Equal("", vm.InspectorFeatureIdText);
        Assert.Equal("", vm.InspectorFeaturePointCountText);
        Assert.Equal("", vm.InspectorFeatureClosedText);
        Assert.Equal("", vm.InspectorFeatureStatusText);
    }
}
