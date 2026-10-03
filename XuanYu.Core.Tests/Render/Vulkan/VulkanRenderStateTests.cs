using XuanYu.Core.Math;
using XuanYu.Core.Map;
using XuanYu.Core.Space;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render;

namespace XuanYu.Core.Tests.Render.Vulkan;

public sealed class VulkanRenderStateTests
{
    [Fact]
    public void Projection_change_classifies_map_and_gpu_work()
    {
        var before = Projection(MapRenderSnapshot.Empty);
        var after = Projection(new MapRenderSnapshot("map", 100, 80,
            MapSurfaceKind.Flat, 0, 0, 1, 1, 2));

        var changes = VulkanRenderChangeConsumer.Compare(before, after);

        Assert.True(changes.CacheInvalidation.HasFlag(VulkanCacheInvalidation.MapSurface));
        Assert.True(changes.UploadRequired.HasFlag(VulkanUploadRequirement.MapSurface));
        Assert.True(changes.FrameDirty);
    }

    [Fact]
    public void Frame_state_exposes_one_render_origin_for_projection_consumers()
    {
        var camera = new RenderCameraProjection(
            new Vector3d(20_000, 30_000, 40_000), new(0, 0, -1), new(0, 1, 0),
            60, 0.1, 100_000, 1);
        var state = new VulkanFrameState();
        state.Apply(Projection(MapRenderSnapshot.Empty) with { Camera = camera });

        var view = state.GetViewProjection(new(1920, 1080), 1);

        Assert.Equal(camera.Position, view.RenderOrigin);
        Assert.Equal(view.RenderOrigin, state.RenderOrigin);
    }

    [Fact]
    public void Upload_completion_clears_gpu_upload_but_keeps_frame_dirty_until_recorded()
    {
        var changes = new VulkanRenderChangeSet(
            VulkanCacheInvalidation.Terrain,
            VulkanUploadRequirement.Terrain,
            true);
        var resources = VulkanGpuResourceState.From(changes);

        resources.MarkUploadsConsumed();

        Assert.Equal(VulkanUploadRequirement.None, resources.UploadRequired);
        Assert.True(resources.FrameDirty);
        resources.MarkFrameRecorded();
        Assert.False(resources.FrameDirty);
    }

    static RenderProjection Projection(MapRenderSnapshot map) => new(
        new RenderCameraProjection(new(0, 0, 10), new(0, 0, -1), new(0, 1, 0),
            60, 0.1, 1000, 1), [], false, default, Map: map);
}
