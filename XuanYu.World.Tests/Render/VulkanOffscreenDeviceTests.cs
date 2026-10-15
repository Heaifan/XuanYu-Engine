using Silk.NET.Vulkan;
using XuanYu.Render.Vulkan;
using XuanYu.Render.Vulkan.Device;
using XuanYu.Render.Vulkan.Render.Readback;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Session;
using XuanYu.Editor.UI;
using XuanYu.Core.Math;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanOffscreenDeviceTests
{
    [Fact]
    public void Creates_graphics_device_without_surface_present_or_swapchain_extension()
    {
        using var vk = Vk.GetApi();
        using var instance = VulkanInstanceOwner.CreateOffscreen(vk);
        var selection = VulkanPhysicalDeviceSelector.SelectOffscreen(vk, instance.Instance);

        Assert.True(selection.Success, selection.Message);
        Assert.NotNull(selection.Queue);
        Assert.True(selection.Queue.HasGraphics);
        Assert.False(selection.Queue.HasPresent);

        using var device = VulkanDeviceOwner.CreateOffscreen(vk, selection);
        Assert.NotNull(device);
        Assert.True(device.LogicalDevice.Handle != 0);
        using var target = VulkanOffscreenColorTarget.Create(vk, device, 64, 64);
        Assert.Equal(64u, target.Extent.Width);
        Assert.Equal(64u, target.Extent.Height);
        Assert.True(target.Image.Handle != 0);
    }

    [Fact]
    public void Renders_terrain_projection_to_offscreen_gpu_image()
    {
        const int side = 33;
        var heights = new double[side * side];
        for (var y = 0; y < side; y++)
        for (var x = 0; x < side; x++)
        {
            var dx = x - side / 2d; var dy = y - side / 2d;
            heights[y * side + x] = 300 + 900 * Math.Exp(-(dx * dx + dy * dy) / 80);
        }
        var tile = TerrainElevationTile.Create(side, side, heights,
            new TerrainGeoBounds(24, 121, 25, 122), new TerrainResolution(30, 30));
        var terrain = tile.ToRenderSnapshot("offscreen-test", 1, side);
        var camera = new RenderCameraProjection(new Vector3d(480, 480, 2000),
            new Vector3d(0, 0, -1), new Vector3d(0, 1, 0), 45, 0.1, 10000, 7);
        var projection = new RenderProjection(camera, [], false, default, Terrain: terrain);

        using var renderer = new VulkanOffscreenTerrainRenderer(projection, 128, 128);
        using var cancelled = new CancellationTokenSource();
        cancelled.Cancel();
        Assert.Throws<OperationCanceledException>(() => renderer.RenderRgba(cancelled.Token));
        var pixels = renderer.RenderRgba();
        if (renderer.GpuTimestampSupported)
            Assert.True(renderer.LastGpuRenderMilliseconds is >= 0);
        Assert.Equal(128 * 128 * 4, pixels.Length);
        Assert.True(pixels.Chunk(4).Select(pixel => string.Join(',', pixel)).Distinct().Count() > 10);
    }
}
