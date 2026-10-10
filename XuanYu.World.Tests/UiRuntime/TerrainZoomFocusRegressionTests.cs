using System.Buffers.Binary;
using XuanYu.Core.Math;
using XuanYu.Core.Space;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using Xunit.Abstractions;

namespace XuanYu.World.Tests.UiRuntime;

public sealed class TerrainZoomFocusRegressionTests(ITestOutputHelper output)
{
    [Fact]
    public async Task Real_dem_focus_anchor_stays_centered_through_zoom_sequence()
    {
        var vm = new UiVm(null, () => true, seedInitialScene: false);
        vm.UpdateViewportFrame(800, 600);
        var path = WriteDem();
        try { Assert.True(await vm.ImportTerrainSourceAsync(path)); }
        finally { File.Delete(path); }
        vm.RunCommand.Execute("查看全部");

        var anchor = TerrainCenter(vm);
        var camera = vm.RenderSnapshot.CameraState;
        Assert.InRange((camera.Position - anchor).Cross(camera.Forward).Length, 0, 1e-8);
        var viewport = vm.CurrentViewport;
        var expected = ViewProjectionState.Create(camera, viewport).ProjectWorldPoint(anchor);
        var previous = vm.RenderSnapshot.CameraState;
        foreach (var targetDistance in new[] { 1.0, 5.0, 20.0, 100.0, 500.0, 1000.0, 2000.0, 5000.0, 10_000.0 })
        {
            var currentDistance = previous.Position.DistanceTo(vm.ObservationCenter);
            var wheelDelta = System.Math.Log(targetDistance / currentDistance) / System.Math.Log(0.85);
            Assert.True(vm.DollyCamera(wheelDelta));
            var next = vm.RenderSnapshot.CameraState;
            var delta = next.Position - previous.Position;
            output.WriteLine($"TargetDistance={targetDistance}; ActualDistance={next.Position.DistanceTo(vm.ObservationCenter):0.######}; " +
                $"Dot={delta.Normalize().Dot(previous.Forward):0.############}; Cross={delta.Normalize().Cross(previous.Forward).Length:0.############}; " +
                $"PivotRayError={(vm.ObservationCenter - next.Position).Cross(next.Forward).Length:0.############}");
            var state = ViewProjectionState.Create(next, viewport);
            var actual = state.ProjectWorldPoint(anchor);
            Assert.Equal(expected.X, actual.X, 5);
            Assert.Equal(expected.Y, actual.Y, 5);
            Assert.InRange((vm.ObservationCenter - next.Position).Cross(next.Forward).Length, 0, 1e-8);
            previous = next;
        }
    }

    static Vector3d TerrainCenter(UiVm vm)
    {
        var points = vm.RenderProjection.Projection.TerrainResources.SelectMany(resource =>
        {
            var field = resource.Heightfield;
            var x = (field.Width - 1) * resource.CellSizeMeters;
            var y = (field.Height - 1) * resource.CellSizeYMeters;
            var min = field.ElevationMeters.Min(); var max = field.ElevationMeters.Max();
            return new[] { new Vector3d(resource.WorldOrigin.X, resource.WorldOrigin.Y, min),
                new Vector3d(resource.WorldOrigin.X + x, resource.WorldOrigin.Y + y, max) };
        }).ToArray();
        return new((points.Min(p => p.X) + points.Max(p => p.X)) / 2,
            (points.Min(p => p.Y) + points.Max(p => p.Y)) / 2,
            (points.Min(p => p.Z) + points.Max(p => p.Z)) / 2);
    }

    static string WriteDem()
    {
        var path = Path.Combine(Path.GetTempPath(), $"n23e121-zoom-{Guid.NewGuid():N}.hgt");
        var values = new short[] { 10, 20, 30, 40 };
        var bytes = new byte[2];
        using var stream = File.Create(path);
        foreach (var value in values)
        {
            BinaryPrimitives.WriteInt16BigEndian(bytes, value);
            stream.Write(bytes);
        }
        return path;
    }
}
