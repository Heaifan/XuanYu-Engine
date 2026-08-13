namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionSnapPipelineContractTests
{
    [Fact]
    public void Pipeline_uses_release_extent_and_existing_commit_path()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor.UI", "Vm", "Map");
        var helper = File.ReadAllText(Path.Combine(root, "UiVm.MapGeometryEditing.Helpers.cs"));
        var main = File.ReadAllText(Path.Combine(root, "UiVm.MapGeometryEditing.cs"));
        Assert.Contains("RegionSnapPipeline.Resolve", helper);
        Assert.Contains("new RegionEdgeSnapSettings(8, 12)", helper);
        Assert.Contains("MapSession.EditRegionVertices", main);
        Assert.DoesNotContain("EdgeSpatialIndex", helper + main);
    }

    [Fact]
    public void Pipeline_does_not_add_edge_specific_history_or_dataset_writes()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "XuanYu.Editor", "MapEditing");
        var files = Directory.GetFiles(root, "RegionSnap*.cs").Select(File.ReadAllText).ToArray();
        var text = string.Join('\n', files);
        Assert.DoesNotContain("EditRegionVertices", text);
        Assert.DoesNotContain("History", text);
        Assert.DoesNotContain("EntityId", text);
    }

    [Fact]
    public void Release_uses_existing_preview_and_geometry_commit_path()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Vm", "Map", "UiVm.MapGeometryEditing.cs");
        var text = File.ReadAllText(path);
        Assert.Contains("PreviewMapGeometryPointer(x, y, viewport)", text);
        Assert.Contains("MapSession.EditRegionVertices", text);
        Assert.DoesNotContain("EdgeSnapHistory", text);
    }

    [Fact]
    public void Cancel_clears_snap_state_without_a_second_undo_path()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
            "XuanYu.Editor.UI", "Vm", "Map", "UiVm.MapGeometryEditing.cs");
        var text = File.ReadAllText(path);
        Assert.Contains("_regionVertexSnap.Clear();", text);
        Assert.DoesNotContain("SnapUndo", text);
        Assert.DoesNotContain("SnapRedo", text);
    }
}
