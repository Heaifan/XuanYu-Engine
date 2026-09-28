using XuanYu.Core.Math;
using XuanYu.Render.Abstractions;
using XuanYu.World.Geo;
using XuanYu.World.Terrain;
using XuanYu.World.Terrain.Source;

namespace XuanYu.Editor.UI;

static class TerrainWorldPlacement
{
    public static TerrainRenderResource ToRenderResource(
        TerrainElevationTile tile, TerrainGeoBounds aggregate, TerrainWorld? world = null)
    {
        var resource = world?.ToRenderSnapshot(tile.TileId, 1) ?? tile.ToRenderSnapshot(tile.TileId, 1);
        var origin = new GeographicPosition(
            aggregate.South, aggregate.West, 0);
        var mapping = new GeographicWorldMapping(origin);
        var worldOrigin = mapping.ToWorld(new(tile.Bounds.South, tile.Bounds.West, 0));
        return resource with { WorldOrigin = new Vector3d(worldOrigin.X, worldOrigin.Y, 0) };
    }
}
