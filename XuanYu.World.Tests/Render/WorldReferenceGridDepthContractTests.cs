using XuanYu.Render.Abstractions;
using XuanYu.Core.Map;

namespace XuanYu.World.Tests.Render;

public sealed class WorldReferenceGridDepthContractTests
{
    [Fact]
    public void Reference_grid_uses_depth_test_without_writing_depth()
    {
        var grid = Read("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Grid.cs");
        var fullscreen = Read("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Fullscreen.cs");

        Assert.Contains("ShaderBytecodeWorldReferenceGridFrag.Code", grid);
        Assert.Contains("VulkanClearFrameOwner.ReferenceGridPushSize, log, depthTest: true", grid);
        Assert.Contains("DepthTestEnable = depthTest", fullscreen);
        Assert.Contains("DepthWriteEnable = false", fullscreen);
        Assert.Contains("DepthCompareOp = CompareOp.GreaterOrEqual", fullscreen);
        Assert.Contains("BlendEnable = true", fullscreen);
    }

    [Fact]
    public void Navigation_gizmo_and_world_origin_keep_depth_off()
    {
        var grid = Read("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Grid.cs");

        Assert.Contains("ShaderBytecodeWorldOriginFrag.Code", grid);
        Assert.Contains("ShaderBytecodeNavGizmoFrag.Code", grid);
        Assert.Equal(3, grid.Split("depthTest: false").Length - 1);
    }

    [Fact]
    public void Draw_plan_keeps_terrain_and_map_before_reference_grid()
    {
        var terrain = new TerrainRenderResource("terrain", 1,
            new TerrainHeightfield(2, 2, [1, 2, 3, 4]));
        var map = new MapRenderSnapshot("map", 100, 100, MapSurfaceKind.Flat, 0, 0, 1, 1, 1);
        var projection = new RenderProjection(default, [], false, default,
            Assist: new EditorViewportAssistState(true, true, false, true),
            Map: map, Terrain: terrain);
        var kinds = RenderDrawPlan.GetFrameDrawPlan(projection).Select(entry => entry.Kind).ToArray();

        Assert.Equal([
            RenderDrawKind.EditorBackground, RenderDrawKind.Terrain,
            RenderDrawKind.MapGround, RenderDrawKind.MapBounds,
            RenderDrawKind.EditorReferenceGrid, RenderDrawKind.WorldOrigin,
            RenderDrawKind.NavigationGizmo], kinds);
    }

    static string Read(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var path = Path.Combine([dir.FullName, .. parts]);
            if (File.Exists(path)) return File.ReadAllText(path);
            dir = dir.Parent;
        }
        throw new FileNotFoundException(string.Join("/", parts));
    }
}
