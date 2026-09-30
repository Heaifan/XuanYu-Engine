using System.Buffers.Binary;
using XuanYu.Core.Space;
using XuanYu.Editor.MapEditing;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;
using XuanYu.World.Map;

namespace XuanYu.World.Tests.Terrain;

public sealed partial class TerrainAuthoringContinuityTests
{
    static void AssertTerrainAuthoringFrame(UiVm vm, ViewportState viewport, int draftVertices)
    {
        var result = vm.RenderProjection;
        Assert.True(result.Success, result.FailureReason);
        var projection = result.Projection;
        var terrainEntries = RenderDrawPlan.GetFrameDrawPlan(projection)
            .Where(entry => entry.Kind == RenderDrawKind.Terrain).ToArray();
        Assert.NotEmpty(terrainEntries);
        Assert.DoesNotContain(RenderDrawPlan.GetFrameDrawPlan(projection),
            entry => entry.Kind == RenderDrawKind.MapGround);
        var state = projection.Camera.ToViewProjection(viewport);
        var visible = projection.TerrainResources.SelectMany(resource =>
            TerrainChunkPartitioner.Partition(resource.Heightfield, resource.TerrainId, resource.Revision)
                .Select(chunk => new XuanYu.Core.Space.TerrainChunkBounds(chunk.ChunkX,
                    new(new(resource.WorldOrigin.X + chunk.WorldBounds.MinX,
                        resource.WorldOrigin.Y + chunk.WorldBounds.MinY, chunk.WorldBounds.MinZ),
                        new(resource.WorldOrigin.X + chunk.WorldBounds.MaxX,
                            resource.WorldOrigin.Y + chunk.WorldBounds.MaxY, chunk.WorldBounds.MaxZ))))
            .Where(chunk => TerrainFrustumCuller.Intersects(state, chunk.Bounds))
            .ToArray());
        Assert.NotEmpty(visible);
        Assert.Equal(draftVertices, vm.RegionDrawingDraftVertexCount);
    }

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
