using System.Numerics;
using XuanYu.Core.Map;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.VectorOverlay;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapVectorOverlayDepthPolicyTests
{
    [Theory]
    [InlineData(0.0, true)]
    [InlineData(45.0, false)]
    [InlineData(80.0, false)]
    [InlineData(89.0, false)]
    public void Clip_policy_orders_fill_stroke_marker_at_extreme_views(double angle, bool orthographic)
    {
        var viewport = new ViewportState(0, 0, 1920, 1080, 1920, 1080, 1, 1);
        var state = ViewProjectionState.Create(Camera(angle, orthographic), viewport);
        var clip = Vector4.Transform(new Vector4(0, 0, 0, 1), state.ViewProjection);
        var fill = VulkanVectorOverlayDepthPolicy.Apply(clip, RenderVectorOverlayPrimitiveKind.Fill);
        var stroke = VulkanVectorOverlayDepthPolicy.Apply(clip, RenderVectorOverlayPrimitiveKind.Stroke);
        var marker = VulkanVectorOverlayDepthPolicy.Apply(clip, RenderVectorOverlayPrimitiveKind.Marker);

        Assert.True(float.IsFinite(clip.W) && clip.W > 0);
        Assert.True(marker.Z < stroke.Z && stroke.Z < fill.Z && fill.Z < clip.Z);
        Assert.Equal(clip.W, marker.W);
        Assert.InRange(marker.Z / marker.W, 0.0f, 1.0f);
    }

    [Fact]
    public void Pipeline_and_shader_contract_keep_depth_test_write_and_draw_order()
    {
        var depth = File.ReadAllText(FindRepoFile("XuanYu.Render.Vulkan", "Pipeline", "VulkanGraphicsPipelineOwner.Depth.cs"));
        var shader = File.ReadAllText(FindRepoFile("XuanYu.Render.Vulkan", "Shaders", "scene.vert"));
        Assert.Contains("DepthTestEnable = true", depth);
        Assert.Contains("DepthWriteEnable = true", depth);
        Assert.Contains("DepthCompareOp = CompareOp.LessOrEqual", depth);
        Assert.Contains("applyVectorOverlayDepthPolicy", shader);
        Assert.Contains("VECTOR_OVERLAY_FILL_DEPTH_BIAS", shader);
        Assert.Contains("VECTOR_OVERLAY_STROKE_DEPTH_BIAS", shader);
        Assert.Contains("VECTOR_OVERLAY_MARKER_DEPTH_BIAS", shader);
        Assert.DoesNotContain("BaseHeightMeters +", shader);
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
