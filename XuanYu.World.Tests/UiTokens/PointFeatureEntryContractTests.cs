using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class PointFeatureEntryContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", rel));

    [Fact]
    public void Workspace_selector_exposes_a_point_feature_entry()
    {
        var selector = Read("Workspace/WorkspaceSelector.axaml");

        Assert.DoesNotContain("点要素编辑", selector);
        Assert.DoesNotContain("OpenPointFeatureEditorCommand", selector);
    }

    [Fact]
    public void Point_feature_entry_projects_to_existing_marker_authoring_chain()
    {
        var selector = Read("Workspace/WorkspaceSelector.axaml");
        var workspace = Read("Vm/Workspace/UiVm.Workspace.cs");
        var authoring = Read("Vm/Workspace/UiVm.RegionAuthoring.cs");

        Assert.Contains("RegionEditor", selector);
        Assert.Contains("SelectRegionAuthoringMode(\"地图标记\")", workspace);
        Assert.Contains("BeginMarkerPlacementAsync", Read("Vm/Map/UiVm.MapDataset.MarkerBootstrap.cs"));
        Assert.Contains("MarkerPlacementPointerPressed", Read("Vm/Map/UiVm.MapMarkerPlacement.cs"));
        Assert.Contains("Marker", authoring);
    }
}
