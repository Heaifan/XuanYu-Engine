using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain;

namespace XuanYu.Editor.UI;

public sealed partial class UiVm
{
    internal int TerrainRenderResourceBuildCount { get; private set; }

    void RebuildTerrainRenderResources()
    {
        if (_terrainTiles is null) { _terrainRenderResources = null; return; }
        _terrainRenderResources = _terrainTiles.Tiles.Select(tile =>
        {
            var world = _terrainTiles.Tiles.Count == 1 ? TerrainWorld : null;
            var resource = TerrainWorldPlacement.ToRenderResource(
                tile, _terrainTiles.Bounds, world);
            var revision = _terrainRevisionLedger.Resolve(resource.TerrainId,
                TerrainContentFingerprint.ForHeightfield(resource.Heightfield));
            return resource with { Revision = revision };
        }).ToArray();
        _terrainRenderEditRevision = TerrainWorld?.EditDelta.Revision ?? 0;
        TerrainRenderResourceBuildCount++;
    }

    IReadOnlyList<TerrainRenderResource>? CachedTerrainRenderResources()
    {
        if (_terrainRenderResources is null) return null;
        if (TerrainWorld is { } world && _terrainRenderEditRevision != world.EditDelta.Revision)
            RebuildTerrainRenderResources();
        return _terrainRenderResources;
    }
}
