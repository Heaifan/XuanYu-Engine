namespace XuanYu.World.Tests.MapEditing;

public sealed class RegionVertexSnapIntegrationContractTests
{
    const string UiRoot = "XuanYu.Editor.UI";
    const string MapRoot = "XuanYu.Editor";

    [Fact]
    public void Preview_path_calls_resolver_without_new_commit_pipeline()
    {
        var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
            UiRoot, "Vm", "Map", "UiVm.MapGeometryEditing.cs"));
        Assert.Contains("ResolveRegionVertexSnap", text);
        Assert.Contains("MapSession.EditRegionVertices", text);
        Assert.DoesNotContain("SnapVertexCommand", text);
        Assert.DoesNotContain("SnapHistory", text);
    }

    [Fact]
    public void Snap_core_has_no_dataset_or_spatial_index_writes()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", MapRoot, "MapEditing");
        var files = Directory.GetFiles(root, "RegionVertexSnap*.cs");
        var text = string.Join('\n', files.Select(File.ReadAllText));
        Assert.DoesNotContain("EditRegionVertices", text);
        Assert.DoesNotContain("Upsert", text);
        Assert.DoesNotContain("Rebuild", text);
    }

    [Fact]
    public void Snap_core_uses_local_query_and_no_map_regions_fallback()
    {
        var root = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", MapRoot, "MapEditing");
        var resolver = File.ReadAllText(Path.Combine(root, "RegionVertexSnapResolver.cs"));
        Assert.Contains("localQuery", resolver);
        Assert.DoesNotContain("map.Regions", resolver);
        Assert.DoesNotContain("AllRegions", resolver);
    }

    [Fact]
    public void F2_drag_cleanup_clears_snap_state_on_commit_and_cancel()
    {
        var text = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
            UiRoot, "Vm", "Map", "UiVm.MapGeometryEditing.cs"));
        Assert.True(text.Split("_regionVertexSnap.Clear()", StringSplitOptions.None).Length >= 4);
        Assert.Contains("_regionVertexSnap.Clear();", text);
    }
}
