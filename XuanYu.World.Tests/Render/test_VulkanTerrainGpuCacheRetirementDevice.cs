using XuanYu.Core.Math;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.Render.Vulkan.Render.Terrain.Offscreen;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Tests.Render;

public sealed class VulkanTerrainGpuCacheRetirementDeviceTests
{
    [Fact]
    public void Evicted_real_vulkan_buffers_stay_alive_until_command_buffers_are_replaced()
    {
        const int side = 242;
        var tile = CreateTile(side);
        var firstTerrain = tile.ToRenderSnapshot("retirement-first", 1, side);
        var nextTerrain = tile.ToRenderSnapshot("retirement-next", 2, side);
        var camera = new RenderCameraProjection(new Vector3d(3615, 3615, 22000),
            new Vector3d(0, 0, -1), new Vector3d(0, 1, 0), 60, 0.1, 100000, 1);
        var first = new RenderProjection(camera, [], false, default, Terrain: firstTerrain);
        using var renderer = new VulkanOffscreenTerrainRenderer(first, 128, 128,
            terrainGpuCacheBudgetBytes: 1);
        Assert.True(OperatingSystem.IsWindows());
        var deviceName = renderer.DeviceName;
        var driver = renderer.DeviceDriverVersion;
        var initialRetired = renderer.RetiredTerrainGpuResourceCount;
        var initialDisposed = renderer.DisposedTerrainGpuResourceCount;
        var diagnostics = Path.Combine(TestSourceRoot.Root, "artifacts", "diagnostics",
            "terrain-gpu-retirement", Guid.NewGuid().ToString("N"));
        WriteSnapshot(diagnostics, "AfterRendererConstructionBeforeFirstReadback",
            deviceName, driver, renderer.TerrainStats.ToString(), initialRetired,
            initialDisposed, renderer.TerrainRetirementDiagnostics);

        Assert.True(initialRetired > 0);
        Assert.Equal(0, initialDisposed);
        Assert.Equal(128 * 128 * 4, renderer.RenderRgba().Length);
        var disposedAfterSubmit = renderer.DisposedTerrainGpuResourceCount;
        WriteSnapshot(diagnostics, "AfterFirstReadback", deviceName, driver,
            renderer.TerrainStats.ToString(), renderer.RetiredTerrainGpuResourceCount,
            disposedAfterSubmit, renderer.TerrainRetirementDiagnostics);
        Assert.Equal(0, disposedAfterSubmit);

        renderer.UpdateProjection(first with { Terrain = nextTerrain });
        var disposedAfterReplacement = renderer.DisposedTerrainGpuResourceCount;
        var retiredAfterReplacement = renderer.RetiredTerrainGpuResourceCount;
        WriteSnapshot(diagnostics, "AfterCommandBufferReplacement", deviceName, driver,
            renderer.TerrainStats.ToString(), retiredAfterReplacement,
            disposedAfterReplacement, renderer.TerrainRetirementDiagnostics);
        Assert.True(disposedAfterReplacement > 0);
        Assert.True(retiredAfterReplacement > 0);
        Assert.Equal(128 * 128 * 4, renderer.RenderRgba().Length);
        Assert.False(string.IsNullOrWhiteSpace(diagnostics));
        File.WriteAllLines(Path.Combine(diagnostics!, "terrain-gpu-cache-retirement-device.txt"),
        [
            $"UTC={DateTime.UtcNow:O}", $"OS={Environment.OSVersion}",
            $"VulkanDevice={deviceName}", $"DriverVersion={driver}", "CacheBudgetBytes=1",
            $"RetiredBeforeSubmit={initialRetired}", $"DisposedBeforeSubmit={initialDisposed}",
            $"DisposedAfterQueueSubmitFenceAndReadback={disposedAfterSubmit}",
            $"DisposedAfterCommandBufferReplacement={disposedAfterReplacement}",
            $"RetiredAfterReplacement={retiredAfterReplacement}", "ReadbackBytes=65536"
        ]);
    }

    static void WriteSnapshot(string? directory, string phase, string device,
        uint driver, string stats, int retired, int disposed, string state)
    {
        if (string.IsNullOrWhiteSpace(directory)) return;
        Directory.CreateDirectory(directory);
        File.WriteAllLines(Path.Combine(directory, $"terrain-gpu-retirement-{phase}.txt"),
        [ $"Phase={phase}", $"Device={device}", $"DriverVersion={driver}",
            $"TerrainStats={stats}", "CacheBudgetBytes=1", $"Retired={retired}",
            $"Disposed={disposed}", state ]);
    }

    static TerrainElevationTile CreateTile(int side)
    {
        var heights = new double[side * side];
        Array.Fill(heights, 300d);
        return TerrainElevationTile.Create(side, side, heights,
            new TerrainGeoBounds(24, 121, 25, 122), new TerrainResolution(30, 30));
    }
}
