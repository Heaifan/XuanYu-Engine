using System.Buffers.Binary;
using XuanYu.Core.Math;
using XuanYu.Editor.UI;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class EditorNavigationCenterContinuityTests
{
    [Fact]
    public void Zoom_then_orbit_keeps_one_center()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        var center = vm.ObservationCenter;
        Assert.True(vm.DollyCamera(1));
        Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));
        Assert.Equal(center, vm.OrbitProbeEvents.Last().Pivot);
        Assert.Equal(center, vm.ObservationCenter);
    }

    [Fact]
    public void Orbit_then_zoom_keeps_one_center()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        var center = vm.ObservationCenter;
        Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(1, 430, 320));
        Assert.True(vm.EndCameraNavigation(1));
        Assert.True(vm.DollyCamera(1));
        Assert.Equal(center, vm.ObservationCenter);
    }

    [Fact]
    public void Pan_then_zoom_and_orbit_share_the_new_center()
    {
        var vm = new UiVm(null, () => true);
        vm.UpdateViewportFrame(800, 600);
        Assert.True(vm.BeginCameraNavigation(1, 400, 300, true, 800, 600));
        Assert.True(vm.PreviewCameraNavigation(1, 450, 330));
        Assert.True(vm.EndCameraNavigation(1));
        var center = vm.ObservationCenter;
        Assert.NotEqual(Vector3d.Zero, center);
        Assert.True(vm.DollyCamera(1));
        Assert.Equal(center, vm.ObservationCenter);
        Assert.True(vm.BeginCameraNavigation(2, 400, 300, false, 800, 600));
        Assert.Equal(center, vm.OrbitProbeEvents.Last().Pivot);
    }

    [Fact]
    public async Task Import_zoom_and_orbit_keep_initial_terrain_framing_center()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var oldCenter = vm.ObservationCenter;
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-{Guid.NewGuid():N}.hgt");
        var bytes = new byte[8];
        for (var i = 0; i < 4; i++) BinaryPrimitives.WriteInt16BigEndian(bytes.AsSpan(i * 2), (short)(i + 1));
        try
        {
            await File.WriteAllBytesAsync(path, bytes);
            Assert.True(await vm.ImportTerrainSourceAsync(path));
            var center = vm.ObservationCenter;
            Assert.NotEqual(oldCenter, center);
            Assert.True(vm.DollyCamera(1));
            Assert.Equal(center, vm.ObservationCenter);
            Assert.True(vm.BeginCameraNavigation(1, 400, 300, false, 800, 600));
            Assert.Equal(center, vm.OrbitProbeEvents.Last().Pivot);
        }
        finally { File.Delete(path); }
    }
}
