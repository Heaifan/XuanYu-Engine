using XuanYu.Render.Abstractions;
using XuanYu.World.Terrain.Source;

namespace XuanYu.World.Terrain;

public static class TerrainElevationTileRuntimeAdapter
{
    public static TerrainRenderResource ToRenderSnapshot(this TerrainElevationTile tile,
        string terrainId, int revision, int maxSamples = 513)
        => TerrainWorld.FromElevationTile(tile).ToRenderSnapshot(terrainId, revision, maxSamples);
}
