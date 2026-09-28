using XuanYu.Render.Abstractions;
using XuanYu.Core.Map;
using System.Text.RegularExpressions;

namespace XuanYu.World.Tests.Render;

public sealed class WorldReferenceGridDepthContractTests
{
    [Fact]
    public void Reference_grid_is_depth_independent_without_writing_depth()
    {
        var grid = Read("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Grid.cs");
        var fullscreen = Read("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Fullscreen.cs");
        var shader = Read("XuanYu.Render.Vulkan", "Shaders", "editor_world_reference_grid.frag");

        var createReferenceGrid = Method(grid, "CreateReferenceGrid");
        Assert.Contains("ShaderBytecodeWorldReferenceGridFrag.Code", createReferenceGrid);
        Assert.Contains("VulkanClearFrameOwner.ReferenceGridPushSize, log", createReferenceGrid);
        Assert.Contains("depthTest: false", createReferenceGrid);
        Assert.DoesNotContain("depthBias", createReferenceGrid);
        Assert.Contains("DepthTestEnable = depthTest", fullscreen);
        Assert.Contains("DepthWriteEnable = false", fullscreen);
        Assert.Contains("DepthCompareOp = CompareOp.GreaterOrEqual", fullscreen);
        Assert.Contains("BlendEnable = true", fullscreen);
        Assert.DoesNotContain("DepthBias", fullscreen);
        Assert.DoesNotContain("gl_FragDepth", shader);
    }

    [Fact]
    public void Non_grid_fullscreen_passes_keep_depth_off()
    {
        var grid = Read("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Grid.cs");

        Assert.Contains("ShaderBytecodeWorldOriginFrag.Code", Method(grid, "CreateWorldOrigin"));
        Assert.Contains("ShaderBytecodeNavGizmoFrag.Code", Method(grid, "CreateNavigationGizmo"));
        Assert.Contains("depthTest: false", Method(grid, "CreateWorldOrigin"));
        Assert.Contains("depthTest: false", Method(grid, "CreateNavigationGizmo"));
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

    static string Method(string source, string name)
    {
        var match = Regex.Match(source, $"{name}\\b(?<body>.*?)(?=\\n    internal static|\\n    //|\\n}})",
            RegexOptions.Singleline);
        Assert.True(match.Success, $"Missing method: {name}");
        return match.Groups["body"].Value;
    }
}
