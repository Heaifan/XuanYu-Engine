using System.Collections.Immutable;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.VectorOverlay;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class MapVectorOverlayV1Tests
{
    [Fact]
    public void V1_R01_one_draft_point_is_a_marker()
    {
        var state = Draft(new MapPoint(-10, -10));
        var resource = MapRegionRenderProjection.Build(MapDefaultDefinition.CreateDefault(), state);
        var marker = Assert.Single(resource.Primitives);
        Assert.Equal(RenderVectorOverlayPrimitiveKind.Marker, marker.Kind);
        Assert.Equal(6.5, marker.RadiusDip);
    }

    [Fact]
    public void V1_R02_two_points_have_stroke_and_markers()
    {
        var state = Draft(new MapPoint(-10, -10), new MapPoint(10, -10));
        var resource = MapRegionRenderProjection.Build(MapDefaultDefinition.CreateDefault(), state);
        Assert.Contains(resource.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Stroke);
        Assert.Equal(2, resource.Primitives.Count(x => x.Kind == RenderVectorOverlayPrimitiveKind.Marker));
    }

    [Fact]
    public void V1_R03_cursor_changes_overlay_revision()
    {
        var state = Draft(new MapPoint(-10, -10));
        state.UpdatePointer(new MapPoint(10, 10), false);
        var first = MapRegionRenderProjection.Build(MapDefaultDefinition.CreateDefault(), state);
        state.UpdatePointer(new MapPoint(20, 20), false);
        var second = MapRegionRenderProjection.Build(MapDefaultDefinition.CreateDefault(), state);
        Assert.NotEqual(first.Revision, second.Revision);
    }

    [Fact]
    public void V1_R04_concave_region_has_fill_triangles_and_closed_stroke()
    {
        var map = MapDefaultDefinition.CreateDefault();
        var region = new MapRegion(MapRegionId.New(), map.Layers[2].LayerId, "凹区域", MapRegionKind.Generic,
            [new(-100, -100), new(100, -100), new(100, 0), new(0, -20), new(100, 100), new(-100, 100)]);
        var resource = MapRegionRenderProjection.Build(map with { Regions = [region] }, new());
        Assert.Contains(resource.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Fill && x.IndexCount >= 12);
        Assert.Contains(resource.Primitives, x => x.Kind == RenderVectorOverlayPrimitiveKind.Stroke);
    }

    [Fact]
    public void V1_R05_R06_shader_expands_width_and_marker_in_pixels()
    {
        var shader = File.ReadAllText(FindRepoFile("XuanYu.Render.Vulkan", "Shaders", "scene.vert"));
        Assert.Contains("pc.gizmoRingRadius * 2.0 / viewport", shader);
        Assert.Contains("selectionMode < 1.5", shader);
    }

    [Fact]
    public void V1_R10_reuses_buffer_only_when_capacity_is_enough()
    {
        Assert.True(VulkanVectorOverlayBufferReusePolicy.CanReuse(128, 64));
        Assert.False(VulkanVectorOverlayBufferReusePolicy.CanReuse(64, 128));
    }

    [Fact]
    public void V1_R08_R09_region_projection_has_no_static_model_resources()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        var resource = MapRegionRenderProjection.Build(MapDefaultDefinition.CreateDefault(),
            Draft(new MapPoint(-10, -10)));
        var projection = SceneRenderProjectionAdapter.TryCreate(vm.RenderSnapshot,
            vectorOverlays: [resource]).Projection;
        Assert.Empty(projection.StaticModelResources);
        Assert.Single(projection.VectorOverlayResources);
    }

    static RegionDrawingState Draft(params MapPoint[] points)
    {
        var map = MapDefaultDefinition.CreateDefault();
        var state = new RegionDrawingState();
        state.Start(map.Layers[2].LayerId, "草稿", MapRegionKind.Generic);
        foreach (var point in points) state.AddVertex(point);
        return state;
    }

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
