using XuanYu.Core.Math;
using XuanYu.Core.Spatial;
using XuanYu.Editor.UI;
using XuanYu.Render.Abstractions;

namespace XuanYu.World.Tests.Camera;

static class TerrainFarPlaneTestBounds
{
    public static SpatialAabb[] For(UiVm vm) => vm.RenderProjection.Projection.TerrainResources
        .SelectMany(resource => TerrainChunkPartitioner.Partition(resource.Heightfield,
            resource.TerrainId, resource.Revision).Select(chunk => Bounds(vm, resource, chunk))).ToArray();

    static SpatialAabb Bounds(UiVm vm, TerrainRenderResource resource, TerrainChunkDescriptor chunk)
    {
        var transform = new TerrainRenderTransform(vm.VerticalExaggeration);
        var skirt = Math.Max(1, Math.Max(resource.Heightfield.CellSizeMeters,
            resource.Heightfield.CellSizeYMeters) * vm.VerticalExaggeration);
        var b = chunk.WorldBounds.WithVerticalExaggeration(transform, skirt);
        return new(new(resource.WorldOrigin.X + b.MinX, resource.WorldOrigin.Y + b.MinY, b.MinZ),
            new(resource.WorldOrigin.X + b.MaxX, resource.WorldOrigin.Y + b.MaxY, b.MaxZ));
    }
}
