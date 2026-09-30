using System.Buffers.Binary;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Terrain;

public sealed partial class TerrainAuthoringContinuityTests
{
    static IReadOnlyList<(double X, double Y)> FindTerrainClicks(UiVm vm,
        ViewportState viewport, int count)
    {
        var projection = ViewProjectionState.Create(vm.RenderSnapshot.Camera!.Value, viewport);
        var surface = new TerrainWorldGroundSurface(vm.TerrainWorld!);
        return Enumerable.Range(1, 19).SelectMany(x => Enumerable.Range(1, 14)
            .Select(y => (X: x * 40.0, Y: y * 40.0)))
            .Where(point =>
            {
                var ray = WorldRayFactory.FromViewportPoint(projection, point.X, point.Y);
                var hit = GroundPickResolver.Resolve(ray, surface, 0);
                return hit.IsValid && !MapBounds.Contains(vm.MapSession.CurrentMap.SizeMeters,
                    hit.WorldXY.X, hit.WorldXY.Y);
            })
            .Take(count).ToArray();
    }

    static void WriteHgt(string path, params short[] values)
    {
        using var stream = File.Create(path);
        Span<byte> sample = stackalloc byte[2];
        foreach (var value in values)
        {
            BinaryPrimitives.WriteInt16BigEndian(sample, value);
            stream.Write(sample);
        }
    }

    sealed class FlatTerrainSurface : IGroundSurface
    {
        public SurfaceBinding Binding => SurfaceBinding.Terrain("dem");
        public int? Revision => 1;
        public bool TryGetElevation(MapPoint point, out double elevation)
        {
            elevation = 25;
            return true;
        }
    }
}
