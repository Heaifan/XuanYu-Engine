using System.IO;

namespace XuanYu.World.Tests.UiTokens;

public sealed class PointFeatureEntryContractTests
{
    static string Read(string rel) => File.ReadAllText(Path.Combine(TestSourceRoot.Root,  "XuanYu.Editor.UI", rel));

    [Fact]
    public void Workspace_selector_exposes_a_point_feature_entry()
    {
        var selector = Read("Panels/Workspace/WorkspaceSelector.axaml") + Read("Panels/Workspace/WorkspaceSelector.axaml.cs");

        Assert.DoesNotContain("点要素编辑", selector);
        Assert.DoesNotContain("OpenPointFeatureEditorCommand", selector);
    }

    [Fact]
    public void Point_feature_entry_projects_to_existing_marker_authoring_chain()
    {
        var selector = Read("Panels/Workspace/WorkspaceSelector.axaml") + Read("Panels/Workspace/WorkspaceSelector.axaml.cs");
        var workspace = Read("Panels/Workspace/WorkspaceLifecycle/UiVm.Workspace.cs");
        var authoring = Read("Panels/Workspace/WorkspaceCommands/UiVm.RegionAuthoring.cs");

        Assert.Contains("SelectFeatureWorkspaceCommand", selector);
        Assert.Contains("SelectRegionAuthoringMode(\"地图标记\")", workspace);
        Assert.Contains("BeginMarkerPlacementAsync", Read("MapUI/Data/DatasetAuthoring/UiVm.MapDataset.MarkerBootstrap.cs"));
        Assert.Contains("MarkerPlacementPointerPressed", Read("MapUI/Interaction/Marker/UiVm.MapMarkerPlacement.cs"));
        Assert.Contains("Marker", authoring);
    }
}
