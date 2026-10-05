using System.Buffers.Binary;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.Input;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Viewport.InputIntegration;

public sealed class TerrainDemCursorZoomRegressionTests
{
    static readonly ViewportState Viewport = new(0, 0, 800, 600, 800, 600, 1, 1);

    [Fact]
    public async Task TerrainZoom_FixedScreenAnchor_PreservesTerrainWorldPoint()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var path = WriteDem();
        try { Assert.True(await vm.ImportTerrainSourceAsync(path)); }
        finally { File.Delete(path); }
        vm.RunCommand.Execute("查看全部");

        Assert.NotNull(vm.TerrainWorld);
        var surface = new TerrainWorldGroundSurface(vm.TerrainWorld!);
        var before = vm.RenderSnapshot.CameraState;
        var hit = FindTerrainHit(before, surface, out var x, out var y);
        var pivot = vm.ObservationCenter;
        Assert.True(vm.DollyCameraAtCursor(1, x, y, Viewport));
        var after = vm.RenderSnapshot.CameraState;
        var next = FindTerrainHit(after, surface, out _, out _, x, y);

        Assert.Equal(SurfaceBindingKind.Terrain, hit.SurfaceBinding.Kind);
        Assert.Equal(SurfaceBindingKind.Terrain, next.SurfaceBinding.Kind);
        var xyError = System.Math.Sqrt(System.Math.Pow(hit.WorldXY.X - next.WorldXY.X, 2) +
            System.Math.Pow(hit.WorldXY.Y - next.WorldXY.Y, 2));
        Assert.InRange(xyError, 0, 1e-5);
        Assert.InRange(System.Math.Abs(hit.ResolvedElevation - next.ResolvedElevation), 0, 1e-5);
        Assert.InRange((before.Forward - after.Forward).Length, 0, 1e-9);
        Assert.Equal(pivot, vm.ObservationCenter);
    }

    static GroundPickResult FindTerrainHit(CameraState camera, IGroundSurface surface,
        out double x, out double y, double? fixedX = null, double? fixedY = null)
    {
        var projection = ViewProjectionState.Create(camera, Viewport);
        foreach (var px in fixedX is { } fx ? new[] { fx } : Enumerable.Range(0, 17).Select(i => i * 50.0))
        foreach (var py in fixedY is { } fy ? new[] { fy } : Enumerable.Range(0, 13).Select(i => i * 50.0))
        {
            var result = GroundPickResolver.Resolve(WorldRayFactory.FromViewportPoint(projection, px, py), surface, 0);
            if (result.IsValid) { x = px; y = py; return result; }
        }
        throw new Xunit.Sdk.XunitException("未找到有效 Terrain Screen Hit");
    }

    static string WriteDem()
    {
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-zoom-{Guid.NewGuid():N}.hgt");
        var bytes = new byte[2]; using var stream = File.Create(path);
        foreach (var value in new short[] { 10, 80, 30, 120 })
        { BinaryPrimitives.WriteInt16BigEndian(bytes, value); stream.Write(bytes); }
        return path;
    }

    static EditorPointerEvent Wheel(double x, double y, double delta) => new(
        EditorPointerEventKind.Wheel, new(x, y), EditorPointerButtons.None,
        EditorPointerModifiers.None, delta, 1, new("test"), 1);
}
