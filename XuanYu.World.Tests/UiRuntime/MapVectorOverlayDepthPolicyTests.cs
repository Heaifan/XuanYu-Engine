using System.Numerics;
using XuanYu.Core.Map;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Camera;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapVectorOverlayDepthPolicyTests
{
    [Fact]
    public void Pipeline_and_shader_contract_disable_depth_and_clip_bias()
    {
        var depth = File.ReadAllText(FindRepoFile("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Depth.cs"));
        var overlay = File.ReadAllText(FindRepoFile("XuanYu.Render.Vulkan", "Session", "VulkanRenderSession.VectorOverlay.cs"));
        var bind = File.ReadAllText(FindRepoFile("XuanYu.Render.Vulkan", "Render", "ClearFrame", "VulkanClearFrameOwner.PipelineBind.cs"));
        var shader = File.ReadAllText(FindRepoFile("XuanYu.Render.Vulkan", "Shaders", "scene.vert"));
        Assert.Contains("DepthTestEnable = depthTest", depth);
        Assert.Contains("DepthWriteEnable = depthWrite", depth);
        Assert.Contains("depthTest: false, depthWrite: false", overlay);
        Assert.Contains("kind == RenderDrawKind.MapVectorOverlay", bind);
        Assert.Contains("DepthCompareOp = CompareOp.LessOrEqual", depth);
        Assert.DoesNotContain("applyVectorOverlayDepthPolicy", shader);
        Assert.DoesNotContain("VECTOR_OVERLAY_FILL_DEPTH_BIAS", shader);
        Assert.DoesNotContain("VECTOR_OVERLAY_STROKE_DEPTH_BIAS", shader);
        Assert.DoesNotContain("VECTOR_OVERLAY_MARKER_DEPTH_BIAS", shader);
        Assert.DoesNotContain("BaseHeightMeters +", shader);
    }

    [Theory]
    [InlineData(45.0)]
    [InlineData(80.0)]
    public void Large_map_fill_uses_direct_projection_at_legal_zoom(double angle)
    {
        var viewport = new ViewportState(0, 0, 1920, 1080, 1920, 1080, 1, 1);
        var direction = Camera(angle, false);
        var start = new CameraState(new(0, -10000, 10000), direction.Forward,
            direction.Up, 60, 0.1, 100000, 1);
        var state = ViewProjectionState.Create(start, viewport);
        foreach (var vertex in new[] { new Vector3d(-250, -250, 0), new(250, -250, 0), new(0, 250, 0) })
        {
            var clip = Vector4.Transform(new Vector4((float)vertex.X, (float)vertex.Y,
                (float)vertex.Z, 1), state.ViewProjection);
            Assert.True(float.IsFinite(clip.W) && clip.W > 0);
            Assert.True(float.IsFinite(clip.Z / clip.W));
        }
    }

    [Fact]
    public void Draw_plan_places_overlay_after_ground_and_before_navigation_gizmo()
    {
        var resource = new RenderVectorOverlayResource(new("region"), 1,
            [new(Vector3d.Zero, Vector3d.Zero, 0, 0)], [0, 0, 0],
            [new(0, 3, 0, RenderVectorOverlayPrimitiveKind.Fill, RenderStaticModelColor.Neutral, 0, 0)],
            new XuanYu.Core.Spatial.SpatialAabb(Vector3d.Zero, Vector3d.Zero));
        var map = new MapRenderSnapshot("map", 100, 100, MapSurfaceKind.Flat, 0, 0, 1, 1, 1);
        var plan = RenderDrawPlan.GetFrameDrawPlan(new RenderProjection(default, [], false,
            Vector3d.Zero, VectorOverlays: [resource], Map: map));
        var overlay = IndexOf(plan, RenderDrawKind.MapVectorOverlay);
        Assert.True(overlay > IndexOf(plan, RenderDrawKind.MapGround));
        Assert.True(overlay < IndexOf(plan, RenderDrawKind.NavigationGizmo));
    }

    static CameraState Camera(double angle, bool orthographic)
    {
        if (orthographic) return new(new Vector3d(0, 0, 337.5), -Vector3d.UnitZ,
            Vector3d.UnitY, 60, 0.1, 10000, 1, ProjectionMode.Orthographic, 675);
        var radians = angle * Math.PI / 180.0;
        var position = new Vector3d(0, -200 * Math.Cos(radians), 200 * Math.Sin(radians));
        return new(position, -position, Vector3d.UnitZ, 60, 0.1, 100000, 1);
    }

    static int IndexOf(IReadOnlyList<RenderDrawPlan.FrameEntry> plan, RenderDrawKind kind) =>
        plan.Select((item, index) => (item, index)).Single(x => x.item.Kind == kind).index;

    static string FindRepoFile(params string[] parts)
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var path = Path.Combine([dir.FullName, .. parts]);
            if (File.Exists(path)) return path;
            dir = dir.Parent;
        }
        throw new FileNotFoundException(string.Join("/", parts));
    }
}
